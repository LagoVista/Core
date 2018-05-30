using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class AsyncProxyFactory : IAsyncProxyFactory
    {
        public TProxy Create<TProxy>(
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender,
            ILogger logger,
            //IUsageMetrics usageMetrics,
            TimeSpan timeout)
        {
            if(asyncCoupler == null)
            {
                throw new ArgumentNullException(nameof(asyncCoupler));
            }

            if (requestSender == null)
            {
                throw new ArgumentNullException(nameof(asyncCoupler));
            }

            return AsyncProxy.CreateProxy<TProxy>(
                asyncCoupler, 
                requestSender,
                logger,
                //usageMetrics,
                timeout);
        }
    }
}
