using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.Client.ServiceBus
{
    public abstract class AbstractProxyClient : ITransceiver
    {
        #region Protected Fields
        protected readonly IAsyncCoupler<IMessage> _asyncCoupler;
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

        #region Rcp Receiver Methods

        /// <summary>
        /// receives responses from the server
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(IMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var invokeResult = await _asyncCoupler.CompleteAsync(message.CorrelationId, message);

            if (!invokeResult.Successful)
            {
                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                    throw new RpcException(FormatErrorMessage(error, "AsyncCoupler failed to complete message with error:"));
            }
        }

        /// <summary>
        /// starts listening for responses from server
        /// </summary>
        public abstract void Start();

        #endregion

        #region Rpc Transmitter Methods

        /// <summary>
        /// transmits requests and waits on response
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<IMessage> TransmitAsync(IMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            await CustomTransmitMessageAsync(message);

            var invokeResult = await _asyncCoupler.WaitOnAsync(message.CorrelationId, _requestTimeout);
            
            // timeout is the only likely failure case
            if (!invokeResult.Successful)
            {
                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                    throw new RpcException(FormatErrorMessage(error, "AsyncCoupler failed to complete message with error:"));
            }

            return invokeResult.Result;
        }

        protected abstract Task CustomTransmitMessageAsync(IMessage message);
        

        #endregion

        protected string FormatErrorMessage(ErrorMessage error, string prefix)
        {
            return $"{prefix} {error.ErrorCode}, {error.Message}, system error ? {error.SystemError}, {error.Details}";
        }
    }
}
