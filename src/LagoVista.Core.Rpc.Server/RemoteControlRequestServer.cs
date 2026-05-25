using LagoVista.Core;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server.RemoteControl
{
    public sealed class RemoteControlRequestServer : IRcgRuntimeCommandHandler
    {
        private readonly IRequestBroker _requestBroker;
        private readonly ILogger _logger;

        public RemoteControlRequestServer(IRequestBroker requestBroker, ILogger logger)
        {
            _requestBroker = requestBroker ?? throw new ArgumentNullException(nameof(requestBroker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RemoteControlFrame> HandleAsync(RcgRuntimeCommandContext context, CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Frame == null) throw new ArgumentNullException(nameof(context.Frame));

            try
            {
                if (String.IsNullOrWhiteSpace(context.CorrelationId))
                {
                    return CreateErrorFrame(context.Frame, "missing_correlation_id", "Remote control RPC request did not include a correlation id.");
                }

                if (String.IsNullOrWhiteSpace(context.PayloadBase64))
                {
                    return CreateErrorFrame(context.Frame, "missing_payload", "Remote control RPC request did not include a payload.");
                }

                _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Received RPC request from Remote Control Gateway.", context.CorrelationId.ToKVP("CorrelationId"), context.Method.ToKVP("Method"));

                var requestBytes = Convert.FromBase64String(context.PayloadBase64);
                var request = new Request(requestBytes);
                var response = await _requestBroker.InvokeAsync(request);

                if (response == null)
                {
                    return CreateErrorFrame(context.Frame, "missing_response", "Remote control RPC request broker did not return a response.");
                }

                if (response.Payload == null)
                {
                    return CreateErrorFrame(context.Frame, "missing_response_payload", "Remote control RPC response did not include a payload.");
                }

                _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Returning RPC response to Remote Control Gateway.", context.CorrelationId.ToKVP("CorrelationId"), context.Method.ToKVP("Method"), response.Success.ToString().ToKVP("RpcSuccess"));

                return new RemoteControlFrame
                {
                    FrameType = RemoteControlFrameTypes.Response,
                    CorrelationId = context.CorrelationId,
                    Method = context.Method,
                    ContentType = typeof(Response).FullName,
                    PayloadBase64 = Convert.ToBase64String(response.Payload)
                };
            }
            catch (Exception ex)
            {
                _logger.AddException(this.Tag(), ex, context.CorrelationId.ToKVP("CorrelationId"), context.Method.ToKVP("Method"));
                return CreateErrorFrame(context.Frame, ex.GetType().Name, ex.Message);
            }
        }

        private static RemoteControlFrame CreateErrorFrame(RemoteControlFrame requestFrame, string errorCode, string errorMessage)
        {
            if (requestFrame == null) throw new ArgumentNullException(nameof(requestFrame));

            return new RemoteControlFrame
            {
                FrameType = RemoteControlFrameTypes.Error,
                CorrelationId = requestFrame.CorrelationId,
                Method = requestFrame.Method,
                ContentType = requestFrame.ContentType,
                ErrorCode = errorCode ?? String.Empty,
                ErrorMessage = errorMessage ?? String.Empty
            };
        }
    }
}
