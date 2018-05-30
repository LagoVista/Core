using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncProxyFactory
    {
        TProxy Create<TProxy>(
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender,
            ILogger logger,
            string destination,
            //todo: ML - get usage metrics added back
            //IUsageMetrics usageMetrics,
            TimeSpan timeout);
    }
}
