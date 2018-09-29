using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server
{
    public abstract class AbstractRequestServer : ITransceiver
    {
        protected readonly ITransceiverConnectionSettings _connectionSettings;
        private readonly IRequestBroker _requestBroker;
        protected readonly ILogger _logger;

        public AbstractRequestServer(ITransceiverConnectionSettings connectionSettings, IRequestBroker requestBroker, ILogger logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _requestBroker = requestBroker ?? throw new ArgumentNullException(nameof(requestBroker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private bool _started = false;

        public void Start()
        {
            if (_started)
            {
                return;
            }

            CustomStart();
            _started = true;
        }

        public async Task TransmitAsync(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await CustomTransmitMessageAsync(message);
        }

        public async Task ReceiveAsync(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var response = await _requestBroker.InvokeAsync((IRequest)message);
            await TransmitAsync(response);
        }

        protected abstract void CustomStart();
        protected abstract Task CustomTransmitMessageAsync(IMessage message);
    }
}