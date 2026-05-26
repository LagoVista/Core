using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Security;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Services
{
    public partial class RcgRuntimeChannelClient : IRcgRuntimeChannelClient
    {
        private readonly RcgRuntimeChannelSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly SignedRequestHeaderBuilder _headerBuilder;

        public RcgRuntimeChannelClient(RcgRuntimeChannelSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ValidateSettings(_settings);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(EnsureTrailingSlash(_settings.GatewayBaseUrl)),
                Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds <= 0 ? 30 : _settings.TimeoutSeconds)
            };

            _headerBuilder = new SignedRequestHeaderBuilder();
        }

        public async Task<InvokeResult<RemoteControlWelcome>> RequestChannelAsync(EntityHeader org, EntityHeader user)
        {
            if (EntityHeader.IsNullOrEmpty(org)) return InvokeResult<RemoteControlWelcome>.FromError("Organization is required.");
            if (EntityHeader.IsNullOrEmpty(user)) return InvokeResult<RemoteControlWelcome>.FromError("User is required.");

            var request = new RemoteControlSessionRequest
            {
                TargetId = _settings.TargetId,
                TargetInstanceId = _settings.TargetInstanceId,
                OrganizationId = _settings.OrganizationId,
                TargetType = _settings.TargetType,
                ProtocolVersion = _settings.ProtocolVersion
            };

            var requestJson = JsonConvert.SerializeObject(request);
            var headers = _headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                Key = _settings.RuntimeKey,
                RequestId = Guid.NewGuid().ToId(),
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                OrganizationId = _settings.OrganizationId,
                Organization = org.Text,
                UserId = _settings.UserId,
                User = user.Text,
                InstanceId = _settings.TargetInstanceId,
                Instance = String.IsNullOrWhiteSpace(_settings.TargetId) ? _settings.TargetInstanceId : _settings.TargetId
            });

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/remote-control/sessions"))
            {
                httpRequest.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                foreach (var header in headers)
                {
                    if (!httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        httpRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                using (var response = await _httpClient.SendAsync(httpRequest))
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError($"Remote Control Gateway returned HTTP {(int)response.StatusCode}: {responseText}");
                    }

                    if (String.IsNullOrWhiteSpace(responseText))
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError("Remote Control Gateway returned an empty welcome response.");
                    }

                    var invokeResult = JsonConvert.DeserializeObject<InvokeResult<RemoteControlWelcome>>(responseText);
                    if (invokeResult == null)
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError("Remote Control Gateway welcome response could not be deserialized.");
                    }

                    if (!invokeResult.Successful)
                    {
                        return invokeResult;
                    }

                    var welcome = invokeResult.Result;
                    if (welcome == null)
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError("Remote Control Gateway welcome response did not include a result.");
                    }

                    if (String.IsNullOrWhiteSpace(welcome.SessionId))
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError("Remote Control Gateway welcome response did not include a session id.");
                    }

                    if (String.IsNullOrWhiteSpace(welcome.SessionToken))
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError("Remote Control Gateway welcome response did not include a session token.");
                    }

                    if (String.IsNullOrWhiteSpace(welcome.StreamUrl))
                    {
                        return InvokeResult<RemoteControlWelcome>.FromError("Remote Control Gateway welcome response did not include a stream URL.");
                    }

                    return InvokeResult<RemoteControlWelcome>.Create(welcome);
                }
            }
        }

        public async Task<InvokeResult<RcgRuntimeChannelConnection>> ConnectAsync(EntityHeader org, EntityHeader user, CancellationToken cancellationToken)
        {
            var welcomeResult = await RequestChannelAsync(org, user);
            if (!welcomeResult.Successful)
            {
                return InvokeResult<RcgRuntimeChannelConnection>.FromInvokeResult(welcomeResult.ToInvokeResult());
            }

            var welcome = welcomeResult.Result;
            var socketUri = BuildSocketUri(welcome);
            Console.WriteLine($"Connecting to remote control channel at {socketUri}...");
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(socketUri, cancellationToken);

            return InvokeResult<RcgRuntimeChannelConnection>.Create(new RcgRuntimeChannelConnection
            {
                Welcome = welcome,
                Socket = socket
            });
        }

        private Uri BuildSocketUri(RemoteControlWelcome welcome)
        {
            if (welcome == null) throw new ArgumentNullException(nameof(welcome));
            if (String.IsNullOrWhiteSpace(welcome.StreamUrl)) throw new InvalidOperationException("Remote Control Gateway welcome response did not include a stream URL.");
            if (String.IsNullOrWhiteSpace(welcome.SessionId)) throw new InvalidOperationException("Remote Control Gateway welcome response did not include a session id.");
            if (String.IsNullOrWhiteSpace(welcome.SessionToken)) throw new InvalidOperationException("Remote Control Gateway welcome response did not include a session token.");

            var builder = Uri.IsWellFormedUriString(welcome.StreamUrl, UriKind.Absolute)
                ? new UriBuilder(welcome.StreamUrl)
                : new UriBuilder(new Uri(new Uri(EnsureTrailingSlash(_settings.GatewayBaseUrl)), welcome.StreamUrl.TrimStart('/')));

            if (String.Equals(builder.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                builder.Scheme = "wss";
                builder.Port = -1;
            }
            else if (String.Equals(builder.Scheme, "http", StringComparison.OrdinalIgnoreCase))
            {
                builder.Scheme = "ws";
                builder.Port = -1;
            }

            var queryPrefix = String.IsNullOrWhiteSpace(builder.Query) ? String.Empty : builder.Query.TrimStart('?') + "&";
            builder.Query = queryPrefix + "sessionId=" + Uri.EscapeDataString(welcome.SessionId) + "&token=" + Uri.EscapeDataString(welcome.SessionToken);

            return builder.Uri;
        }

        private static void ValidateSettings(RcgRuntimeChannelSettings settings)
        {
            if (String.IsNullOrWhiteSpace(settings.GatewayBaseUrl)) throw new ArgumentNullException(nameof(settings.GatewayBaseUrl));
            if (String.IsNullOrWhiteSpace(settings.RuntimeKey)) throw new ArgumentNullException(nameof(settings.RuntimeKey));
            if (String.IsNullOrWhiteSpace(settings.TargetId)) throw new ArgumentNullException(nameof(settings.TargetId));
            if (String.IsNullOrWhiteSpace(settings.TargetInstanceId)) throw new ArgumentNullException(nameof(settings.TargetInstanceId));
            if (String.IsNullOrWhiteSpace(settings.OrganizationId)) throw new ArgumentNullException(nameof(settings.OrganizationId));
            if (String.IsNullOrWhiteSpace(settings.UserId)) throw new ArgumentNullException(nameof(settings.UserId));
            if (String.IsNullOrWhiteSpace(settings.ProtocolVersion)) throw new ArgumentNullException(nameof(settings.ProtocolVersion));
        }

        private static string EnsureTrailingSlash(string uri)
        {
            if (String.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri));

            return uri.EndsWith("/", StringComparison.Ordinal) ? uri : uri + "/";
        }
    }
}
