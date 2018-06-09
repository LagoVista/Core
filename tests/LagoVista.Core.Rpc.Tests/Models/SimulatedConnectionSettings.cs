using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Settings;

namespace LagoVista.Core.Rpc.Tests.Models
{
    public sealed class SimulatedConnectionSettings : ITransceiverConnectionSettings
    {
        public SimulatedConnectionSettings() : base()
        {
            RpcTransmitter = new ConnectionSettings();
            RpcReceiver = new ConnectionSettings();
        }

        public IConnectionSettings RpcTransmitter { get; }
        public IConnectionSettings RpcReceiver { get; }
    }
}
