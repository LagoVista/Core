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
        private readonly string _sourceEntityPath;
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly IAsyncRequestHandler _requestSender;
        private readonly ILogger _logger;
        
        public AsyncProxyFactory(
            IServiceBusAsyncResponseListenerConnectionSettings connectionSettings,
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender,
            ILogger logger)
        {
            Console.WriteLine("AsyncProxyFactory::ctor >>");

            if (connectionSettings == null)
            {
                Console.WriteLine($"AsyncProxyFactory::ctor {nameof(connectionSettings)} is null");
                throw new ArgumentNullException(nameof(connectionSettings));
            }

            // SourceEntityPath - ResourceName
            _sourceEntityPath = connectionSettings.ServiceBusAsyncResponseListener.ResourceName;
            _asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            _requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Console.WriteLine("AsyncProxyFactory::ctor <<");
        }

        public TProxy Create<TProxy>(
            string organizationId,
            string instanceId,
            TimeSpan timeout)
        {
            Console.WriteLine("AsyncProxyFactory::Create >>");
            Console.WriteLine($"AsyncProxyFactory::Create org {organizationId}, inst {instanceId}");
            
            // CreateProxy does null checks
            return AsyncProxy.CreateProxy<TProxy>(
                _asyncCoupler,
                _requestSender,
                _logger,
                organizationId,
                instanceId,
                _sourceEntityPath,
                timeout);
        }
    }
}
