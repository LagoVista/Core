using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Rpc.Tests.Utils;

namespace LagoVista.Core.Rpc.Tests.Models
{
    public sealed class SimulatedConnectionSettings : ITransceiverConnectionSettings
    {
        public SimulatedConnectionSettings() : base()
        {
            RpcClientTransmitter = new ConnectionSettings();
            RpcClientReceiver = new ConnectionSettings
            {
                Uri = Constants.MessageReplyPath,
                ResourceName = Constants.MessageInBox
            };

            RpcClientTransmitter.TimeoutInSeconds = Constants.TimeoutInSeconds;
        }

        public IConnectionSettings RpcClientTransmitter { get; }
        public IConnectionSettings RpcClientReceiver { get; }
        public IConnectionSettings RpcAdmin { get; }

        public IConnectionSettings RpcServerTransmitter { get; }

        public IConnectionSettings RpcServerReceiver { get; }
    }
}
