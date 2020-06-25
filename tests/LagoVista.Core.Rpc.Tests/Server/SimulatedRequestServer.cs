using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Rpc.Tests.Middelware;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    public sealed class SimulatedRequestServer : AbstractRequestServer
    {
        private readonly QueueSimulator _queue;

        public SimulatedRequestServer(
            IRequestBroker requestBroker,
            ILogger logger,
            QueueSimulator queue) : base(requestBroker, logger)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        protected override void ConfigureSettings(ITransceiverConnectionSettings settings)
        {

        }

        protected override Task CustomStartAsync()
        {
            _queue.RegisterListener(this, QueueSimulator.ListenerType.Request);
            return Task.FromResult<object>(null);
        }

        protected override async Task<InvokeResult> CustomTransmitMessageAsync(IMessage message)
        {
            await _queue.SendAsync((IResponse)message);
            return InvokeResult.Success;
        }

        protected override void UpdateSettings(ITransceiverConnectionSettings settings)
        {

        }
    }
}
