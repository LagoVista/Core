using LagoVista.Core.Interfaces;
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

        public SignedRequestPublicKeySetHttpClient(HttpClient httpClient, IConfigServerClientSettings settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<SignedRequestPublicKeySet> FetchAsync(CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(_settings.PublicKeySetUrl)) throw new InvalidOperationException("PublicKeySetUrl is required.");

            using (var request = new HttpRequestMessage(HttpMethod.Get, _settings.PublicKeySetUrl))
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
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AuthorizationToken);
                }

                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException($"Public key set request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).");
                    }

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

                    return keySet;
                }
            }
        }
    }
}
