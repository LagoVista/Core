// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 27d4b532d71ba8a7478bc635ddc87a53b3ddd0e277200f8dd197603df35c6bb9
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server
{
    public abstract class AbstractRequestServer : ITransceiver
    {
        private readonly IRequestBroker _requestBroker;
        protected readonly ILogger _logger;

        public AbstractRequestServer(IRequestBroker requestBroker, ILogger logger)
        {
            _requestBroker = requestBroker ?? throw new ArgumentNullException(nameof(requestBroker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public bool IsRunning { get; private set; } = false;

        public async Task StartAsync(ITransceiverConnectionSettings connectionSettings)
        {
            if(connectionSettings == null) throw new ArgumentNullException(nameof(connectionSettings));

            if (IsRunning)
            {
                return;
            }

            ConfigureSettings(connectionSettings);

            await CustomStartAsync();
            IsRunning = true;
        }        

        public async Task<InvokeResult> TransmitAsync(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return await CustomTransmitMessageAsync(message);
        }

        public async Task<InvokeResult> ReceiveAsync(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var response = await _requestBroker.InvokeAsync((IRequest)message);
            return await TransmitAsync(response);
        }

        protected abstract void ConfigureSettings(ITransceiverConnectionSettings settings);
        protected abstract void UpdateSettings(ITransceiverConnectionSettings settings);


        protected abstract Task CustomStartAsync();
        protected abstract Task<InvokeResult> CustomTransmitMessageAsync(IMessage message);


        public Task RefreshConnection(ITransceiverConnectionSettings connectionSettings)
        {
            UpdateSettings(connectionSettings);
            return Task.FromResult(default(object));
        }

    }
}