using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Configuration
{
    public class RemoteConfigurationClient : IRemoteConfigurationClient
    {
        private static readonly int[] RetryDelaysMs = { 0, 500, 1500 };

        private readonly HttpClient _httpClient;

        public RemoteConfigurationClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IConfigurationRoot> LoadAsync(RemoteConfigurationSettings settings, string appKey, string environmentKey, CancellationToken cancellationToken = default)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (String.IsNullOrWhiteSpace(appKey)) throw new ArgumentException("App key is required.", nameof(appKey));
            if (String.IsNullOrWhiteSpace(environmentKey)) throw new ArgumentException("Environment key is required.", nameof(environmentKey));

            settings.Validate();

            var requestUri = $"{settings.ConfigurationServiceBaseUrl.TrimEnd('/')}/api/config/{Uri.EscapeDataString(appKey)}/{Uri.EscapeDataString(environmentKey)}";
            Exception lastException = null;

            for (var attempt = 0; attempt < RetryDelaysMs.Length; attempt++)
            {
                if (RetryDelaysMs[attempt] > 0)
                {
                    await Task.Delay(RetryDelaysMs[attempt], cancellationToken).ConfigureAwait(false);
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                using (var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    request.Headers.Add("x-config-auth", settings.AuthorizationToken);
                    request.Headers.Accept.ParseAdd("application/json");
                    timeoutCts.CancelAfter(TimeSpan.FromMilliseconds(settings.TimeoutMs));

                    try
                    {
                        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, timeoutCts.Token).ConfigureAwait(false))
                        {
                            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            if (!response.IsSuccessStatusCode)
                            {
                                if (ShouldRetry(response.StatusCode) && attempt < RetryDelaysMs.Length - 1)
                                {
                                    lastException = new InvalidOperationException(CreateHttpError(response.StatusCode, response.ReasonPhrase, content));
                                    continue;
                                }

                                throw new InvalidOperationException(CreateHttpError(response.StatusCode, response.ReasonPhrase, content));
                            }

                            RemoteConfigurationResponse remoteResponse;
                            try
                            {
                                remoteResponse = JsonSerializer.Deserialize<RemoteConfigurationResponse>(content, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                            }
                            catch (JsonException ex)
                            {
                                throw new InvalidOperationException("Unable to deserialize remote configuration response.", ex);
                            }

                            ValidateResponse(remoteResponse, appKey, environmentKey);

                            return new ConfigurationBuilder()
                                .Add(new RemoteConfigurationSource(remoteResponse.Values))
                                .Build();
                        }
                    }
                    catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                    {
                        lastException = new TimeoutException($"Timed out after {settings.TimeoutMs}ms while loading remote configuration.", ex);
                        if (attempt < RetryDelaysMs.Length - 1)
                        {
                            continue;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        lastException = ex;
                        if (attempt < RetryDelaysMs.Length - 1)
                        {
                            continue;
                        }
                    }
                }
            }

            throw new InvalidOperationException($"Unable to load remote configuration for app '{appKey}' and environment '{environmentKey}' after {RetryDelaysMs.Length} attempts.", lastException);
        }

        private static void ValidateResponse(RemoteConfigurationResponse response, string appKey, string environmentKey)
        {
            if (response == null)
            {
                throw new InvalidOperationException("Remote configuration response was empty.");
            }

            if (!String.Equals(appKey, response.AppKey, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Remote configuration response app key '{response.AppKey}' did not match requested app key '{appKey}'.");
            }

            if (!String.Equals(environmentKey, response.DeploymentKey, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Remote configuration response environment key '{response.DeploymentKey}' did not match requested environment key '{environmentKey}'.");
            }

            if (response.Values == null)
            {
                throw new InvalidOperationException("Remote configuration response did not contain a values collection.");
            }
        }

        private static bool ShouldRetry(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.BadGateway ||
                   statusCode == HttpStatusCode.ServiceUnavailable ||
                   statusCode == HttpStatusCode.GatewayTimeout;
        }

        private static string CreateHttpError(HttpStatusCode statusCode, string reasonPhrase, string content)
        {
            var body = String.IsNullOrWhiteSpace(content) ? String.Empty : $" {content}";
            return $"Remote configuration request failed with status code {(int)statusCode} ({statusCode}) - {reasonPhrase}.{body}";
        }

        private sealed class RemoteConfigurationSource : IConfigurationSource
        {
            private readonly IDictionary<string, string> _values;

            public RemoteConfigurationSource(IDictionary<string, string> values)
            {
                _values = values ?? throw new ArgumentNullException(nameof(values));
            }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new RemoteConfigurationProvider(_values);
            }
        }

        private sealed class RemoteConfigurationProvider : ConfigurationProvider
        {
            public RemoteConfigurationProvider(IDictionary<string, string> values)
            {
                Data = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
