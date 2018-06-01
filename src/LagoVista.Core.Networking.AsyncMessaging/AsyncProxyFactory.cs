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
            string destinationInstructions,
            TimeSpan timeout)
        {
            // CreateProxy does null checks
            return AsyncProxy.CreateProxy<TProxy>(
                asyncCoupler,
                requestSender,
                logger,
                destinationInstructions,
                timeout);
        }
    }
}
