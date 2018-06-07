using LagoVista.Core.Networking.Rpc.Middleware;
using LagoVista.Core.Networking.Rpc.Settings;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.Rpc.ServiceBus.Client
{
    public sealed class ProxyFactory : IProxyFactory
    {
        private readonly ITransceiverConnectionSettings _connectionSettings;
        private readonly ITransceiver _client;
        private readonly ILogger _logger;

        public ProxyFactory(
            ITransceiverConnectionSettings connectionSettings,
            ITransceiver client,
            ILogger logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TProxyInterface Create<TProxyInterface>() where TProxyInterface : class
        {
            return Proxy.Create<TProxyInterface>(_connectionSettings, _client, _logger);
        }
    }
}
