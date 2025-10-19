// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a6483ca33f01deb0fabd55500d60c2892643cc2680191519e61ab248e8941a0b
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using System;

namespace LagoVista.Core.Rpc.Client
{
    public sealed class ProxyFactory : IProxyFactory
    {
        private readonly ITransceiverConnectionSettings _connectionSettings;
        private readonly ITransceiver _client;
        private readonly IAsyncCoupler<IMessage> _asyncCoupler;
        private readonly ILogger _logger;

        public ProxyFactory(
            ITransceiverConnectionSettings connectionSettings,
            ITransceiver client,
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TProxyInterface Create<TProxyInterface>(IProxySettings proxySettings) where TProxyInterface : class
        {
            return Proxy.Create<TProxyInterface>(_connectionSettings, _client, _asyncCoupler, _logger, proxySettings);
        }
    }
}
