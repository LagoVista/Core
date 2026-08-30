using LagoVista.Core.Models.Configuration;
using System;
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

        public async Task<ResolvedConfiguration> LoadAsync(RemoteConfigurationSettings settings, string appKey, string deploymentKey, CancellationToken cancellationToken = default)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (String.IsNullOrWhiteSpace(appKey)) throw new ArgumentException("App key is required.", nameof(appKey));
            if (String.IsNullOrWhiteSpace(deploymentKey)) throw new ArgumentException("Deployment key is required.", nameof(deploymentKey));

            settings.Validate();

            var requestUri = $"{settings.ConfigurationServiceBaseUrl.TrimEnd('/')}/api/config/{Uri.EscapeDataString(appKey)}/{Uri.EscapeDataString(deploymentKey)}";
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

                            ResolvedConfiguration resolved;
                            try
                            {
                                resolved = JsonSerializer.Deserialize<ResolvedConfiguration>(content, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                            }
                            catch (JsonException ex)
                            {
                                throw new InvalidOperationException("Unable to deserialize remote configuration response.", ex);
                            }

                            if (resolved == null)
                            {
                                throw new InvalidOperationException("Remote configuration response was empty.");
                            }

                            if (!String.Equals(appKey, resolved.AppKey, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new InvalidOperationException($"Remote configuration response app key '{resolved.AppKey}' did not match requested app key '{appKey}'.");
                            }

                            if (!String.Equals(deploymentKey, resolved.DeploymentKey, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new InvalidOperationException($"Remote configuration response deployment key '{resolved.DeploymentKey}' did not match requested deployment key '{deploymentKey}'.");
                            }

                            if (resolved.Values == null)
                            {
                                throw new InvalidOperationException("Remote configuration response did not contain a values collection.");
                            }

                            return resolved;
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

            throw new InvalidOperationException($"Unable to load remote configuration for app '{appKey}' and deployment '{deploymentKey}' after {RetryDelaysMs.Length} attempts.", lastException);
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
    }
}
