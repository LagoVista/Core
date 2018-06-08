using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Rpc.Tests.RequestProxyTests
{
    public sealed class FakeConnectionSettings : IServiceBusResponseListenerConnectionSettings
    {
        private static readonly string _replyPath = "replyPath";

        public IConnectionSettings ServiceBusResponseListener { get; private set; } = new ConnectionSettings()
        {
            ResourceName = _replyPath
        };
    }
}
