using LagoVista.Core.Interfaces;
using LagoVista.Core.Rpc.Settings;

namespace LagoVista.Core.Rpc.Tests.Server.Utils
{
    internal class TransceiverSettings : ITransceiverConnectionSettings
    {
        public IConnectionSettings RpcAdmin { get; set; }

        public IConnectionSettings RpcClientTransmitter { get; set; }

        public IConnectionSettings RpcClientReceiver { get; set; }

        public IConnectionSettings RpcServerTransmitter { get; set; }

        public IConnectionSettings RpcServerReceiver { get; set; }
    }
}
