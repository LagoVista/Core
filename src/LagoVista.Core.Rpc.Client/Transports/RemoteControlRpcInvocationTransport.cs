using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Rpc.Client.Interfaces;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client.Transports
{
    public class RemoteControlRpcInvocationTransport : IRpcInvocationTransport
    {
        private readonly IRcgRpcClientTransport _rcgRpcClientTransport;
        private readonly EntityHeader _user;
        private readonly ILogger _logger;

        public RemoteControlRpcInvocationTransport(IRcgRpcClientTransport rcgRpcClientTransport, ISystemUsers sysUsers, ILogger logger)
        {
            _rcgRpcClientTransport = rcgRpcClientTransport ?? throw new ArgumentNullException(nameof(rcgRpcClientTransport));
            _user = sysUsers?.HostUser ?? throw new ArgumentNullException(nameof(sysUsers));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InvokeResult<IResponse>> InvokeAsync(IRequest request, TimeSpan timeout)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (String.IsNullOrWhiteSpace(request.InstanceId)) return InvokeResult<IResponse>.FromError("RPC request instance id is required for Remote Control Gateway transport.");
            if (String.IsNullOrWhiteSpace(request.CorrelationId)) return InvokeResult<IResponse>.FromError("RPC request correlation id is required for Remote Control Gateway transport.");
            if (request.Payload == null) return InvokeResult<IResponse>.FromError("RPC request payload is required for Remote Control Gateway transport.");

            var timeoutSeconds = Convert.ToInt32(Math.Ceiling(timeout.TotalSeconds));
            if (timeoutSeconds <= 0)
            {
                timeoutSeconds = 30;
            }

            var transportRequest = new RcgRpcClientTransportRequest
            {
                TargetInstanceId = request.InstanceId,
                TimeoutSeconds = timeoutSeconds,
                Frame = new RemoteControlFrame
                {
                    FrameType = RemoteControlFrameTypes.Command,
                    CorrelationId = request.CorrelationId,
                    Method = request.DestinationPath,
                    ContentType = typeof(Request).FullName,
                    PayloadBase64 = Convert.ToBase64String(request.Payload)
                }
            };

            _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Invoking RPC request through Remote Control Gateway.", request.InstanceId.ToKVP("TargetInstanceId"), request.CorrelationId.ToKVP("CorrelationId"), request.DestinationPath.ToKVP("Method"), timeoutSeconds.ToString().ToKVP("TimeoutSeconds"));

            var requestOrg = EntityHeader.Create(request.OrganizationId, "REQUESTORG");

            var transportResult = await _rcgRpcClientTransport.SendAsync(transportRequest, requestOrg, _user);
            if (!transportResult.Successful)
            {
                _logger.AddCustomEvent(LogLevel.Error, this.Tag(), transportResult.ErrorMessage, request.InstanceId.ToKVP("TargetInstanceId"), request.CorrelationId.ToKVP("CorrelationId"), request.DestinationPath.ToKVP("Method"));
                return InvokeResult<IResponse>.FromInvokeResult(transportResult.ToInvokeResult());
            }

            if (transportResult.Result == null)
            {
                return InvokeResult<IResponse>.FromError("Remote Control Gateway RPC response did not include a result.");
            }

            if (transportResult.Result.Frame == null)
            {
                return InvokeResult<IResponse>.FromError("Remote Control Gateway RPC response did not include a frame.");
            }

            var responseFrame = transportResult.Result.Frame;
            if (String.Equals(responseFrame.FrameType, RemoteControlFrameTypes.Error, StringComparison.OrdinalIgnoreCase))
            {
                var errorMessage = String.IsNullOrWhiteSpace(responseFrame.ErrorMessage) ? "Remote Control Gateway RPC invocation returned an error frame." : responseFrame.ErrorMessage;
                _logger.AddCustomEvent(LogLevel.Error, this.Tag(), errorMessage, request.InstanceId.ToKVP("TargetInstanceId"), responseFrame.CorrelationId.ToKVP("CorrelationId"), responseFrame.Method.ToKVP("Method"));
                return InvokeResult<IResponse>.FromError(errorMessage);
            }

            if (!String.Equals(responseFrame.FrameType, RemoteControlFrameTypes.Response, StringComparison.OrdinalIgnoreCase))
            {
                return InvokeResult<IResponse>.FromError($"Remote Control Gateway returned unexpected frame type '{responseFrame.FrameType}'.");
            }

            if (String.IsNullOrWhiteSpace(responseFrame.PayloadBase64))
            {
                return InvokeResult<IResponse>.FromError("Remote Control Gateway RPC response frame did not include a payload.");
            }

            var responseBytes = Convert.FromBase64String(responseFrame.PayloadBase64);
            var response = new Response(responseBytes);

            _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "RPC request completed through Remote Control Gateway.", request.InstanceId.ToKVP("TargetInstanceId"), responseFrame.CorrelationId.ToKVP("CorrelationId"), responseFrame.Method.ToKVP("Method"));

            return InvokeResult<IResponse>.Create(response);
        }
    }
}
