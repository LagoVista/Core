// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 382e377fe2d81d19590890dd322486378b0537766f3aad37593f5b89c3a74e7d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.Diagnostics;
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
    public abstract class AbstractProxyClient : ITransceiver, IHostedServiceDiagnostics
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

        public string Name => $"RPCProxyClient";

        /// <summary>
        /// receives responses from the server
        /// </summary>
        public async Task<InvokeResult> ReceiveAsync(IMessage message)
        {
            if (message == null)  throw new ArgumentNullException(nameof(message));

            _logger.AddCustomEvent(LogLevel.Message, "[ServiceBusPRoxyClient__ProcessMessageAsync]", $"[AbstractProxyClient__ReceiveAsync] - cid: {message.CorrelationId} aid: {_asyncCoupler.InstanceId}", _asyncCoupler.InstanceId.ToKVP("aid"));

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

            try
            {
                await CustomStartAsync();
                IsRunning = true;
                _snapShot.Status = HostedServiceDiagnosticStatus.Running;
                _snapShot.StartedUtc = DateTime.UtcNow;
                _snapShot.LastActivity = "Started listening";
                _snapShot.LastActivityUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _snapShot.LastError = ex.Message;
                _snapShot.LastErrorUtc = DateTime.UtcNow;
                _snapShot.Status = HostedServiceDiagnosticStatus.Error;
            }
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

        protected HostedServiceDiagnosticSnapshot _snapShot = new HostedServiceDiagnosticSnapshot();

        public HostedServiceDiagnosticSnapshot GetSnapshot()
        {
            return _snapShot;
        }
    }
}
