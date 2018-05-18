using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public abstract class AsyncRequestListener : IAsyncRequestListener
    {
        protected readonly IAsyncRequestBroker _requestBroker;
        protected readonly IConnectionSettings _connectionSettings;
        protected readonly ILogger _logger;

        public AsyncRequestListener(IAsyncRequestBroker requestBroker, IConnectionSettings connectionSettings, ILogger logger) : base()
        {
            _requestBroker = requestBroker ?? throw new ArgumentNullException("requestBroker");
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException("connectionSettings");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        public abstract void Start();
    }
}
