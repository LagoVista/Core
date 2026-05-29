using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeySetHttpClient : ISignedRequestPublicKeySetClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigServerClientSettings _settings;
        private readonly ILogger _logger;

        public SignedRequestPublicKeySetHttpClient(HttpClient httpClient, IConfigServerClientSettings settings, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SignedRequestPublicKeySet> FetchAsync(CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(_settings.PublicKeySetUrl)) throw new InvalidOperationException("PublicKeySetUrl is required.");

            _logger.Trace($"{this.Tag()} Fetching signed request public key set from {_settings.PublicKeySetUrl}.");

            var getKeyUrl = $"{_settings.PublicKeySetUrl}/api/config/{_settings.AppKey}/{_settings.Environment}/service-signing/public-keys";

            using (var request = new HttpRequestMessage(HttpMethod.Get, getKeyUrl))
            {

                if (!String.IsNullOrWhiteSpace(_settings.AppKey))
                {
                    request.Headers.TryAddWithoutValidation("X-Nuviot-App-Key", _settings.AppKey);
                }

                if (!String.IsNullOrWhiteSpace(_settings.Environment))
                {
                    request.Headers.TryAddWithoutValidation("X-Nuviot-Environment", _settings.Environment);
                }

                if (!String.IsNullOrWhiteSpace(_settings.AuthorizationToken))
                {
                    request.Headers.TryAddWithoutValidation("X-Config-Auth", _settings.AuthorizationToken);
                }

                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.AddCustomEvent(LogLevel.Error, this.Tag(), $"Public key set request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).");
                        throw new InvalidOperationException($"Public key set request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).");
                    }

                    _logger.Trace($"{this.Tag()} Successfully fetched signed request public key set. Deserializing response.");

                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (String.IsNullOrWhiteSpace(json))
                    {
                        throw new InvalidOperationException("Public key set response was empty.");
                    }

                    var keySet = JsonConvert.DeserializeObject<SignedRequestPublicKeySet>(json);
                    if (keySet == null)
                    {
                        throw new InvalidOperationException("Public key set response could not be deserialized.");
                    }

                    if (keySet.Keys == null)
                    {
                        throw new InvalidOperationException("Public key set response did not include a keys collection.");
                    }


                    _logger.Trace($"{this.Tag()} Found {keySet.Keys.Count} keys.");

                    return keySet;
                }
            }
        }
    }
}
