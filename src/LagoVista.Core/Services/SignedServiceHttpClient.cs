using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Security;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Services
{
    public class SignedServiceHttpClient : ISignedServiceHttpClient
    {
        private readonly ISignedServiceHttpClientSettings _settings;
        private readonly IEnvironmentEndpoints _environmentEndpoints;
        private readonly HttpClient _httpClient;
        private readonly SignedRequestHeaderBuilder _headerBuilder;
        private readonly ILogger _logger;

        public SignedServiceHttpClient(ISignedServiceHttpClientSettings settings, ILogger logger, IEnvironmentEndpoints environmentEndpoints)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environmentEndpoints = environmentEndpoints ?? throw new ArgumentNullException(nameof(environmentEndpoints));
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds <= 0 ? 30 : _settings.TimeoutSeconds)
            };
            _headerBuilder = new SignedRequestHeaderBuilder();
        }

        public Task<InvokeResult<TResult>> GetAsync<TResult>(SignedServiceHttpTarget target, string pathAndQuery)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));

            return SendAsync<TResult>(target, HttpMethod.Get, pathAndQuery, String.Empty);
        }

        public Task<InvokeResult<TResult>> PostAsync<TRequest, TResult>(SignedServiceHttpTarget target, string pathAndQuery, TRequest request)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));

            var json = JsonConvert.SerializeObject(request);
            return SendAsync<TResult>(target, HttpMethod.Post, pathAndQuery, json);
        }

        private async Task<InvokeResult<TResult>> SendAsync<TResult>(SignedServiceHttpTarget target, HttpMethod method, string pathAndQuery, string bodyJson)
        {
            ValidateSigningSettings();

            var baseUrl = _environmentEndpoints.GetBaseUrl(target);
            if (String.IsNullOrWhiteSpace(baseUrl))
            {
                return InvokeResult<TResult>.FromError($"Base URL was not configured for signed service HTTP target '{target}'.");
            }

            var normalizedPath = NormalizePath(pathAndQuery);
            var requestUri = new Uri(new Uri(EnsureTrailingSlash(baseUrl)), normalizedPath.TrimStart('/'));
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson ?? String.Empty);
            var headers = _headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                RequestId = Guid.NewGuid().ToId(),
                DateUtc = DateTimeOffset.UtcNow,
                Version = String.IsNullOrWhiteSpace(_settings.Version) ? "1" : _settings.Version,
                AppKey = _settings.AppKey,
                SigningKeyId = _settings.ServiceSigningKeyId,
                PrivateKeyMaterial = _settings.ServiceSigningPrivateKeyMaterial,
                Method = method.Method,
                PathAndQuery = normalizedPath,
                Body = bodyBytes
            });
            
            _logger.Trace($"{this.Tag()} Sending {method} request to {requestUri}");

            using (var request = new HttpRequestMessage(method, requestUri))
            {
                if (method != HttpMethod.Get)
                {
                    request.Content = new StringContent(bodyJson ?? String.Empty, Encoding.UTF8, "application/json");
                }

                foreach (var header in headers)
                {
                    if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value) && request.Content != null)
                    {
                        request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.AddCustomEvent(LogLevel.Error, this.Tag(), $"Received HTTP {(int)response.StatusCode} response from {requestUri}");
                        return InvokeResult<TResult>.FromError($"Signed service HTTP target '{target}' returned HTTP {(int)response.StatusCode}: {responseText}");
                    }

                    _logger.Trace($"{this.Tag()} Received successful response from {requestUri}. Deserializing response.");

                    if (String.IsNullOrWhiteSpace(responseText))
                    {
                        return InvokeResult<TResult>.FromError($"Signed service HTTP target '{target}' returned an empty response.");
                    }

                    var invokeResult = JsonConvert.DeserializeObject<InvokeResult<TResult>>(responseText);
                    if (invokeResult == null)
                    {
                        _logger.AddCustomEvent(LogLevel.Error, this.Tag(), $"Signed service HTTP target '{target}' response could not be deserialized.");
                        return InvokeResult<TResult>.FromError($"Signed service HTTP target '{target}' response could not be deserialized.");
                    }

                    return invokeResult;
                }
            }
        }

        private void ValidateSigningSettings()
        {
            if (String.IsNullOrWhiteSpace(_settings.AppKey)) throw new InvalidOperationException("AppKey is required for signed service HTTP requests.");
            if (String.IsNullOrWhiteSpace(_settings.ServiceSigningKeyId)) throw new InvalidOperationException("ServiceSigningKeyId is required for signed service HTTP requests.");
            if (String.IsNullOrWhiteSpace(_settings.ServiceSigningPrivateKeyMaterial)) throw new InvalidOperationException("ServiceSigningPrivateKeyMaterial is required for signed service HTTP requests.");
            if (!String.Equals(_settings.ServiceSigningAlgorithm, SignedRequestSignatureAlgorithms.RsaPssSha256, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException($"Unsupported service signing algorithm '{_settings.ServiceSigningAlgorithm}'.");
            if (!String.Equals(_settings.ServiceSigningKeyMaterialFormat, SignedRequestKeyMaterialFormats.RsaXml, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException($"Unsupported service signing key material format '{_settings.ServiceSigningKeyMaterialFormat}'.");
        }

        private static string EnsureTrailingSlash(string uri)
        {
            if (String.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri));

            return uri.EndsWith("/", StringComparison.Ordinal) ? uri : uri + "/";
        }

        private static string NormalizePath(string pathAndQuery)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));

            return pathAndQuery.StartsWith("/", StringComparison.Ordinal) ? pathAndQuery : "/" + pathAndQuery;
        }
    }
}
