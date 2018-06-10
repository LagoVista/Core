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
            RpcTransmitter = new ConnectionSettings();
            RpcReceiver = new ConnectionSettings();
            RpcReceiver.Uri = Constants.MessageReplyPath;
            RpcTransmitter.TimeoutInSeconds = Constants.TimeoutInSeconds;
        }

        public IConnectionSettings RpcTransmitter { get; }
        public IConnectionSettings RpcReceiver { get; }
    }
}
