using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public abstract class RequestListener : IRequestListener
    {
        protected IRequestBroker RequestBroker { get; }
        protected IConnectionSettings ConnectionSettings { get; }
        protected ILogger Logger { get; }

        public RequestListener(IRequestBroker requestBroker, IConnectionSettings connectionSettings, ILogger logger)
        {
            RequestBroker = requestBroker ?? throw new ArgumentNullException("requestBroker");
            ConnectionSettings = connectionSettings ?? throw new ArgumentNullException("connectionSettings");
            Logger = logger ?? throw new ArgumentNullException("logger");
        }

        public abstract void Start();
    }
}
