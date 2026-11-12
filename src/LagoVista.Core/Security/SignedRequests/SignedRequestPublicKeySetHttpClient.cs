using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public interface ISignedRequestKeyDiagnostics
    {
        void Write(string stage, string details);
    }

    public class SignedRequestPublicKeySetHttpClient : ISignedRequestPublicKeySetClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigServerClientSettings _settings;
        private readonly ILogger _logger;
        private readonly ISignedRequestTrustedCallerProvider _trustedCallers;
        private readonly ISignedRequestKeyDiagnostics _diagnostics;

        public SignedRequestPublicKeySetHttpClient(HttpClient httpClient,
            IConfigServerClientSettings settings, ILogger logger,
            ISignedRequestTrustedCallerProvider trustedCallers = null,
            ISignedRequestKeyDiagnostics diagnostics = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _trustedCallers = trustedCallers;
            _diagnostics = diagnostics;
        }

        public async Task<SignedRequestPublicKeySet> FetchAsync(CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(_settings.PublicKeySetUrl))
                throw new InvalidOperationException("PublicKeySetUrl is required.");

            // Always retain local keys for the ordinary signed-service routes.
            var localUrl = $"{_settings.PublicKeySetUrl.TrimEnd('/')}/api/config/{Uri.EscapeDataString(_settings.AppKey)}/{Uri.EscapeDataString(_settings.Environment)}/service-signing/public-keys";
            var local = await FetchSetAsync(localUrl, cancellationToken).ConfigureAwait(false);
            foreach (var key in local.Keys)
                if (key != null) key.SourceEnvironment = _settings.Environment;
            _diagnostics?.Write("local-key-set", $"receiver={_settings.AppKey}/{_settings.Environment} count={local.Keys.Count}");

            var callers = _trustedCallers?.GetTrustedCallers();
            if (callers == null || callers.Count == 0)
            {
                _diagnostics?.Write("trusted-callers", "none registered; cross-environment retrieval skipped");
                return local;
            }

            var allowed = callers.Where(c => c != null &&
                !String.IsNullOrWhiteSpace(c.AppKey) &&
                !String.IsNullOrWhiteSpace(c.Environment)).ToList();

            _diagnostics?.Write("trusted-callers", String.Join(",", allowed.Select(c => c.AppKey + "/" + c.Environment)));

            foreach (var environment in allowed.Select(c => c.Environment)
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (String.Equals(environment, _settings.Environment, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!String.Equals(environment, "dev", StringComparison.OrdinalIgnoreCase) &&
                    !String.Equals(environment, "live", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Unsupported trusted signing environment.");

                // The config auth token always belongs to this receiving app and
                // environment. The special endpoint returns ONLY public keys.
                var url = $"{_settings.PublicKeySetUrl.TrimEnd('/')}/api/config/{Uri.EscapeDataString(_settings.AppKey)}/{Uri.EscapeDataString(_settings.Environment)}/service-signing/trusted-public-keys/{Uri.EscapeDataString(environment)}";
                var remote = await FetchSetAsync(url, cancellationToken).ConfigureAwait(false);
                _diagnostics?.Write("remote-key-set", $"source={environment} returned={remote.Keys.Count} identities={String.Join(",", remote.Keys.Where(k => k != null).Select(k => k.AppKey + "/" + k.KeyId + "/" + k.Status))}");
                if (!String.Equals(remote.Environment, environment, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Cross-environment public key set has an unexpected environment.");

                var accepted = remote.Keys.Where(k => k != null &&
                    allowed.Any(c => String.Equals(c.AppKey, k.AppKey, StringComparison.Ordinal) &&
                    String.Equals(c.Environment, environment, StringComparison.OrdinalIgnoreCase))).ToList();
                _diagnostics?.Write("remote-key-filter", $"source={environment} accepted={accepted.Count} rejected={remote.Keys.Count - accepted.Count} acceptedIds={String.Join(",", accepted.Select(k => k.KeyId))}");
                foreach (var key in accepted)
                {
                    // Never accept an ambiguous key across environments. The global
                    // validator binds AppKey+KeyId; those must be unique across sets.
                    if (local.Keys.Any(x => x != null &&
                        String.Equals(x.AppKey, key.AppKey, StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(x.KeyId, key.KeyId, StringComparison.OrdinalIgnoreCase)))
                        throw new InvalidOperationException("Duplicate signing key identity across environments.");

                    key.SourceEnvironment = environment;
                    local.Keys.Add(key);
                }
            }

            _diagnostics?.Write("effective-key-set", $"receiver={_settings.AppKey}/{_settings.Environment} count={local.Keys.Count} identities={String.Join(",", local.Keys.Where(k => k != null).Select(k => k.AppKey + "/" + k.KeyId + "/" + k.SourceEnvironment))}");
            return local;
        }

        private async Task<SignedRequestPublicKeySet> FetchSetAsync(string url, CancellationToken cancellationToken)
        {
            var path = new Uri(url).AbsolutePath;
            _diagnostics?.Write("key-request", $"path={path}");
            _logger.Trace($"{this.Tag()} Fetching public signing keys from configuration service.");
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation("X-Nuviot-App-Key", _settings.AppKey);
                request.Headers.TryAddWithoutValidation("X-Nuviot-Environment", _settings.Environment);
                if (!String.IsNullOrWhiteSpace(_settings.AuthorizationToken))
                    request.Headers.TryAddWithoutValidation("X-Config-Auth", _settings.AuthorizationToken);

                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    _diagnostics?.Write("key-response", $"path={path} http={(int)response.StatusCode}");
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.AddCustomEvent(LogLevel.Warning, "Security.Diagnostics.CrossEnvironmentSigning", $"Signing key request failed: HTTP {(int)response.StatusCode}, path={path}");
                        throw new InvalidOperationException($"Signing public-key request failed (HTTP {(int)response.StatusCode}).");
                    }
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var set = JsonConvert.DeserializeObject<SignedRequestPublicKeySet>(json);
                    if (set?.Keys == null)
                        throw new InvalidOperationException("Signing public-key response was invalid.");
                    _diagnostics?.Write("key-response-parsed", $"path={path} environment={set.Environment} count={set.Keys.Count}");
                    return set;
                }
            }
        }
    }
}
