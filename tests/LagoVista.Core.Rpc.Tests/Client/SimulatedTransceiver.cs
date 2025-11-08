// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4d2807b841821a691558867f086908d1d7b885a46dc95ccbac92748b83681384
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Client
{
    /// <summary>
    /// for client side only testing of proxy
    /// SimulatedTransceiver "sends" a request, then waits a bit to simulate network and then completes by returning response to receive method
    /// </summary>
    internal sealed class SimulatedTransceiver : AbstractProxyClient
    {
        private readonly object _result;

        public SimulatedTransceiver(
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger,
            object result) :
            base(asyncCoupler, logger)
        {
            _result = result;
        }

        protected override void ConfigureSettings(ITransceiverConnectionSettings settings)
        {
            throw new NotImplementedException();
        }

        protected override Task CustomStartAsync()
        {
            return Task.FromResult<object>(null);
        }

        protected override Task<InvokeResult> CustomTransmitMessageAsync(IMessage message)
        {
            CompleteRequest((IRequest)message, TimeSpan.FromSeconds(1));
            return Task.FromResult<InvokeResult>(InvokeResult.Success);
        }

        private void CompleteRequest(IRequest request, TimeSpan delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(delay);
                var response = new Response(request, _result);
                await ReceiveAsync(response);
            });
        }
    }
}
