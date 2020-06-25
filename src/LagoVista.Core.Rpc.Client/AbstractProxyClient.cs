using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client
{
    public abstract class AbstractProxyClient : ITransceiver
    {
        #region Fields
        private readonly IAsyncCoupler<IMessage> _asyncCoupler;
        protected readonly ILogger _logger;
        #endregion

        #region Constructors

        public AbstractProxyClient(IAsyncCoupler<IMessage> asyncCoupler, ILogger logger) : base()
        {
            _asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region rpc Receiver Methods

        /// <summary>
        /// receives responses from the server
        /// </summary>
        public async Task<InvokeResult> ReceiveAsync(IMessage message)
        {
            if (message == null)  throw new ArgumentNullException(nameof(message));

            return await _asyncCoupler.CompleteAsync(message.CorrelationId, message);
        }

        public bool IsRunning { get; private set; } = false;

        public async Task StartAsync(ITransceiverConnectionSettings connectionSettings)
        {
            if(connectionSettings == null) throw new ArgumentNullException(nameof(connectionSettings));
            ConfigureSettings(connectionSettings);

            if (IsRunning)
            {
                return;
            }

            await CustomStartAsync();
            IsRunning = true;
        }

        protected abstract Task CustomStartAsync();

        #endregion

        protected abstract void ConfigureSettings(ITransceiverConnectionSettings settings);


        public Task RefreshConnection(ITransceiverConnectionSettings connectionSettings)
        {
            ConfigureSettings(connectionSettings);
            return Task.FromResult(default(object));
        }

        #region Rpc Transmitter Methods

        /// <summary>
        /// transmits requests and waits on response
        /// </summary>
        public async Task<InvokeResult> TransmitAsync(IMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return await CustomTransmitMessageAsync(message);
        }

        protected abstract Task<InvokeResult> CustomTransmitMessageAsync(IMessage message);

        #endregion
    }
}
