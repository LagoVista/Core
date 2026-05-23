using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Managers
{
    public class RcgStatusManager : IRcgStatusManager
    {
        private readonly IRcgGatewayHttpClient _gatewayClient;
        private readonly ILogger _adminLogger;

        public RcgStatusManager(IRcgGatewayHttpClient gatewayClient, ILogger adminLogger)
        {
            _gatewayClient = gatewayClient ?? throw new ArgumentNullException(nameof(gatewayClient));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
        }

        public async Task<InvokeResult<RemoteControlDiagnosticsSnapshot>> GetDiagnosticsAsync(EntityHeader org, EntityHeader user)
        {
            const string tag = "[RcgStatusManager__GetDiagnosticsAsync]";

            if (EntityHeader.IsNullOrEmpty(org)) return InvokeResult<RemoteControlDiagnosticsSnapshot>.FromError("Organization is required.");
            if (EntityHeader.IsNullOrEmpty(user)) return InvokeResult<RemoteControlDiagnosticsSnapshot>.FromError("User is required.");

            var result = await _gatewayClient.GetAsync<RemoteControlDiagnosticsSnapshot>("/api/remote-control/diagnostics", org, user);
            if (!result.Successful)
            {
                _adminLogger.AddCustomEvent(LogLevel.Error, tag, result.ErrorMessage);
            }

            return result;
        }
    }
}
