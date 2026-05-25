using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Services
{
    public class RcgChannelSupervisor : IRcgChannelSupervisor
    {
        private readonly IRcgRuntimeChannelClient _channelClient;
        private readonly RcgChannelSupervisorSettings _settings;
        private readonly ILogger _logger;

        public RcgChannelSupervisor(IRcgRuntimeChannelClient channelClient, RcgChannelSupervisorSettings settings, ILogger logger)
        {
            _channelClient = channelClient ?? throw new ArgumentNullException(nameof(channelClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InvokeResult> RunAsync(EntityHeader org, EntityHeader user, IRcgRuntimeCommandHandler handler, CancellationToken cancellationToken)
        {
            if (EntityHeader.IsNullOrEmpty(org)) return InvokeResult.FromError("Organization is required.");
            if (EntityHeader.IsNullOrEmpty(user)) return InvokeResult.FromError("User is required.");
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var reconnectDelay = _settings.GetInitialReconnectDelay();
            var maxReconnectDelay = _settings.GetMaxReconnectDelay();

            _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Starting RCG persistent channel supervisor.", org.Id.ToKVP("OrganizationId"), user.Id.ToKVP("UserId"));

            while (!cancellationToken.IsCancellationRequested)
            {
                RcgRuntimeChannelConnection connection = null;

                try
                {
                    using (var connectTimeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                    {
                        connectTimeoutSource.CancelAfter(_settings.GetConnectTimeout());

                        _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Connecting RCG runtime channel.", reconnectDelay.TotalSeconds.ToString().ToKVP("CurrentReconnectDelaySeconds"));

                        var connectResult = await _channelClient.ConnectAsync(org, user, connectTimeoutSource.Token);
                        if (!connectResult.Successful)
                        {
                            _logger.AddCustomEvent(LogLevel.Error, this.Tag(), connectResult.ErrorMessage);

                            if (_settings.StopOnAuthorizationFailure && IsAuthorizationFailure(connectResult.ErrorMessage))
                            {
                                return connectResult.ToInvokeResult();
                            }

                            await DelayBeforeReconnectAsync(reconnectDelay, cancellationToken);
                            reconnectDelay = IncreaseReconnectDelay(reconnectDelay, maxReconnectDelay);
                            continue;
                        }

                        connection = connectResult.Result;
                    }

                    if (connection == null)
                    {
                        return InvokeResult.FromError("RCG runtime channel connection result did not include a connection.");
                    }

                    reconnectDelay = _settings.GetInitialReconnectDelay();

                    _logger.AddCustomEvent(LogLevel.StateChange, this.Tag(), "RCG runtime channel connected.", connection.Welcome.SessionId.ToKVP("SessionId"));

                    var listenResult = await _channelClient.ListenAsync(connection, handler, cancellationToken);
                    if (!listenResult.Successful)
                    {
                        _logger.AddCustomEvent(LogLevel.Error, this.Tag(), listenResult.ErrorMessage, connection.Welcome.SessionId.ToKVP("SessionId"));
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Warning, this.Tag(), "RCG runtime channel listener exited.", connection.Welcome.SessionId.ToKVP("SessionId"));
                    }
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    _logger.AddCustomEvent(LogLevel.Warning, this.Tag(), "RCG runtime channel connection attempt timed out.");
                }
                catch (Exception ex)
                {
                    _logger.AddException(this.Tag(), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    await DelayBeforeReconnectAsync(reconnectDelay, cancellationToken);
                    reconnectDelay = IncreaseReconnectDelay(reconnectDelay, maxReconnectDelay);
                }
            }

            _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "RCG persistent channel supervisor stopped.");
            return InvokeResult.Success;
        }

        private static bool IsAuthorizationFailure(string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(errorMessage))
            {
                return false;
            }

            return errorMessage.IndexOf("401", StringComparison.OrdinalIgnoreCase) >= 0 || errorMessage.IndexOf("Unauthorized", StringComparison.OrdinalIgnoreCase) >= 0 || errorMessage.IndexOf("authorization", StringComparison.OrdinalIgnoreCase) >= 0 || errorMessage.IndexOf("authentication", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private async Task DelayBeforeReconnectAsync(TimeSpan reconnectDelay, CancellationToken cancellationToken)
        {
            _logger.AddCustomEvent(LogLevel.Warning, this.Tag(), "RCG runtime channel will reconnect after delay.", reconnectDelay.TotalSeconds.ToString().ToKVP("ReconnectDelaySeconds"));
            await Task.Delay(reconnectDelay, cancellationToken);
        }

        private static TimeSpan IncreaseReconnectDelay(TimeSpan currentDelay, TimeSpan maxDelay)
        {
            var nextDelay = TimeSpan.FromSeconds(currentDelay.TotalSeconds * 2);
            return nextDelay > maxDelay ? maxDelay : nextDelay;
        }
    }
}
