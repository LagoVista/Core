using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.RequestProxyTests
{
    public sealed class FakeConnectionSettings : IServiceBusAsyncResponseListenerConnectionSettings
    {
        private static readonly string _replyPath = "replyPath";

        public IConnectionSettings ServiceBusAsyncResponseListener { get; private set; } = new ConnectionSettings()
        {
            ResourceName = _replyPath
        };
    }

}
