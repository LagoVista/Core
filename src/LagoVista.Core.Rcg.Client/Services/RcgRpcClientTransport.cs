using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Services
{
    public class RcgRpcClientTransport : IRcgRpcClientTransport
    {
        private readonly IRcgGatewayHttpClient _gatewayHttpClient;
        private readonly ILogger _logger;

        public RcgRpcClientTransport(IRcgGatewayHttpClient gatewayHttpClient, ILogger adminLogger)
        {
            _gatewayHttpClient = gatewayHttpClient ?? throw new ArgumentNullException(nameof(gatewayHttpClient));
            _logger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
        }

        public async Task<InvokeResult<RcgRpcClientTransportResponse>> SendAsync(RcgRpcClientTransportRequest request, EntityHeader org, EntityHeader user)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (String.IsNullOrWhiteSpace(request.TargetInstanceId)) return InvokeResult<RcgRpcClientTransportResponse>.FromError("Target instance id is required.");
            if (request.Frame == null) return InvokeResult<RcgRpcClientTransportResponse>.FromError("Remote control frame is required.");
            if (EntityHeader.IsNullOrEmpty(org)) return InvokeResult<RcgRpcClientTransportResponse>.FromError("Organization is required.");
            if (EntityHeader.IsNullOrEmpty(user)) return InvokeResult<RcgRpcClientTransportResponse>.FromError("User is required.");

            if (String.IsNullOrWhiteSpace(request.Frame.CorrelationId))
            {
                request.Frame.CorrelationId = Guid.NewGuid().ToString("N");
            }

            if (request.TimeoutSeconds <= 0)
            {
                request.TimeoutSeconds = 30;
            }

            var path = $"/api/rcg/rpc/targets/{request.TargetInstanceId}/frames";
            var result = await _gatewayHttpClient.PostAsync<RcgRpcClientTransportRequest, RcgRpcClientTransportResponse>(path, request, org, user);
            if (!result.Successful)
            {
                _logger.AddCustomEvent(LogLevel.Error, this.Tag(), result.ErrorMessage);
            }

            return result;
        }
    }
}
