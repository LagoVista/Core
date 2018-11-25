using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
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
        protected ITransceiverConnectionSettings _connectionSettings;
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
        public async Task ReceiveAsync(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var invokeResult = await _asyncCoupler.CompleteAsync(message.CorrelationId, message);

            if (!invokeResult.Successful)
            {
                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                {
                    throw new RpcException(RpcException.FormatErrorMessage(error, "AsyncCoupler failed to complete message with error:"));
                }
            }
        }

        public bool IsRunning { get; private set; } = false;

        public async Task StartAsync(ITransceiverConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));

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

        #region Rpc Transmitter Methods

        /// <summary>
        /// transmits requests and waits on response
        /// </summary>
        public async Task TransmitAsync(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await CustomTransmitMessageAsync(message);
        }

        protected abstract Task CustomTransmitMessageAsync(IMessage message);

        #endregion
    }
}
