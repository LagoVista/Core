using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class AsyncProxyFactory : IAsyncProxyFactory
    {
        /// <summary>
        /// the reply to path 
        /// </summary>
        private readonly  string _sourceEntityPath;

        public AsyncProxyFactory(IServiceBusAsyncResponseListenerConnectionSettings connectionSettings)
        {
            /// SourceEntityPath - ResourceName
            _sourceEntityPath = connectionSettings.ServiceBusAsyncResponseListener.ResourceName;
        }

        public TProxy Create<TProxy>(
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender,
            ILogger logger,
            string organizationId,
            string instanceId,
            TimeSpan timeout)
        {
            // CreateProxy does null checks
            return AsyncProxy.CreateProxy<TProxy>(
                asyncCoupler,
                requestSender,
                logger,
                organizationId,
                instanceId,
                _sourceEntityPath,
                timeout);
        }
    }
}
