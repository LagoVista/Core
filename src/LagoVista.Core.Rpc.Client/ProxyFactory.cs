using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client.Interfaces;
using LagoVista.Core.Rpc.Settings;
using System;

namespace LagoVista.Core.Rpc.Client
{
    public sealed class ProxyFactory : IProxyFactory
    {
        private readonly IRpcInvocationTransport _invocationTransport;
        private readonly ILogger _logger;
        private readonly TimeSpan _requestTimeout;

        public ProxyFactory(IRpcInvocationTransport invocationTransport, ILogger logger)
        {
            _invocationTransport = invocationTransport ?? throw new ArgumentNullException(nameof(invocationTransport));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestTimeout = TimeSpan.FromSeconds(30);
        }

        public ProxyFactory(IRpcInvocationTransport invocationTransport, ILogger logger, TimeSpan requestTimeout)
        {
            _invocationTransport = invocationTransport ?? throw new ArgumentNullException(nameof(invocationTransport));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (requestTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentException("Request timeout must be greater than zero.", nameof(requestTimeout));
            }

            _requestTimeout = requestTimeout;
        }

        public TProxyInterface Create<TProxyInterface>(IProxySettings proxySettings) where TProxyInterface : class
        {
            return Proxy.Create<TProxyInterface>(_invocationTransport, _logger, proxySettings, _requestTimeout);
        }
    }
}