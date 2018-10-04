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
        protected readonly ITransceiverConnectionSettings _connectionSettings;
        #endregion

        #region Constructors

        public AbstractProxyClient(ITransceiverConnectionSettings connectionSettings, IAsyncCoupler<IMessage> asyncCoupler, ILogger logger) : base()
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
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

            Console.WriteLine("AbstractProxyClient.ReceiveAsync before _asyncCoupler.CompleteAsync");
            var invokeResult = await _asyncCoupler.CompleteAsync(message.CorrelationId, message);
            Console.WriteLine("AbstractProxyClient.ReceiveAsync after _asyncCoupler.CompleteAsync");

            if (!invokeResult.Successful)
            {
                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                {
                    throw new RpcException(RpcException.FormatErrorMessage(error, "AsyncCoupler failed to complete message with error:"));
                }
            }
        }

        private bool _started = false;
        public async Task StartAsync()
        {
            if (_started)
            {
                return;
            }

            await CustomStartAsync();
            _started = true;
        }

        protected abstract Task CustomStartAsync();

        #endregion

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
