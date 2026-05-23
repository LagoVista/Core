using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Security;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Services
{
    public class RcgGatewayHttpClient : IRcgGatewayHttpClient
    {
        private readonly IRcgClientSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly SignedRequestHeaderBuilder _headerBuilder;

        public RcgGatewayHttpClient(IRcgClientSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (String.IsNullOrWhiteSpace(_settings.GatewayBaseUrl)) throw new ArgumentNullException(nameof(_settings.GatewayBaseUrl));

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(EnsureTrailingSlash(_settings.GatewayBaseUrl)),
                Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds <= 0 ? 30 : _settings.TimeoutSeconds)
            };

            _headerBuilder = new SignedRequestHeaderBuilder();
        }

        public Task<InvokeResult<TResult>> GetAsync<TResult>(string pathAndQuery, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));
            if (EntityHeader.IsNullOrEmpty(org)) return Task.FromResult(InvokeResult<TResult>.FromError("Organization is required."));
            if (EntityHeader.IsNullOrEmpty(user)) return Task.FromResult(InvokeResult<TResult>.FromError("User is required."));

            return SendAsync<TResult>(HttpMethod.Get, pathAndQuery, String.Empty);
        }

        public Task<InvokeResult<TResult>> PostAsync<TRequest, TResult>(string pathAndQuery, TRequest request, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));
            if (EntityHeader.IsNullOrEmpty(org)) return Task.FromResult(InvokeResult<TResult>.FromError("Organization is required."));
            if (EntityHeader.IsNullOrEmpty(user)) return Task.FromResult(InvokeResult<TResult>.FromError("User is required."));

            var json = JsonConvert.SerializeObject(request);
            return SendAsync<TResult>(HttpMethod.Post, pathAndQuery, json);
        }

        private async Task<InvokeResult<TResult>> SendAsync<TResult>(HttpMethod method, string pathAndQuery, string bodyJson)
        {
            var normalizedPath = NormalizePath(pathAndQuery);
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson ?? String.Empty);
            var headers = _headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                Key = _settings.GetSigningKey(),
                RequestId = Guid.NewGuid().ToId(),
                DateUtc = DateTimeOffset.UtcNow,
                Version = String.IsNullOrWhiteSpace(_settings.Version) ? "1" : _settings.Version,
                CallerId = _settings.CallerId,
                Method = method.Method,
                PathAndQuery = normalizedPath,
                Body = bodyBytes
            });

            using (var request = new HttpRequestMessage(method, normalizedPath))
            {
                if (method != HttpMethod.Get)
                {
                    request.Content = new StringContent(bodyJson ?? String.Empty, Encoding.UTF8, "application/json");
                }

                foreach (var header in headers)
                {
                    if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        if (request.Content != null)
                        {
                            request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                }

                using (var response = await _httpClient.SendAsync(request))
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return InvokeResult<TResult>.FromError($"Remote Control Gateway returned HTTP {(int)response.StatusCode}: {responseText}");
                    }

                    if (String.IsNullOrWhiteSpace(responseText))
                    {
                        return InvokeResult<TResult>.FromError("Remote Control Gateway returned an empty response.");
                    }

                    var result = JsonConvert.DeserializeObject<TResult>(responseText);
                    return InvokeResult<TResult>.Create(result);
                }
            }
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
