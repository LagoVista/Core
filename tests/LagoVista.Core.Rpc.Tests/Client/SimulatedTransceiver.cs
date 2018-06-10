using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
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
            ITransceiverConnectionSettings connectionSettings,
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger,
            object result) :
            base(connectionSettings, asyncCoupler, logger)
        {
            _result = result;
        }

        protected override void CustomStart() { }

        protected override Task CustomTransmitMessageAsync(IMessage message)
        {
            CompleteRequest((IRequest)message, TimeSpan.FromSeconds(1));
            return Task.FromResult<object>(null);
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
