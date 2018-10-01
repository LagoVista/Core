using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Rpc.Tests.Middelware;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    public sealed class SimulatedRequestServer : AbstractRequestServer
    {
        private readonly QueueSimulator _queue;

        public SimulatedRequestServer(
            ITransceiverConnectionSettings connectionSettings,
            IRequestBroker requestBroker,
            ILogger logger,
            QueueSimulator queue) : base(connectionSettings, requestBroker, logger)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        protected override Task CustomStartAsync()
        {
            _queue.RegisterListener(this, QueueSimulator.ListenerType.Request);
            return Task.FromResult<object>(null);
        }

        protected override async Task CustomTransmitMessageAsync(IMessage message)
        {
            await _queue.SendAsync((IResponse)message);
        }
    }
}
