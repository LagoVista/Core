// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 033f3f0bcc7ddc068b413a85ec12aceaa60b1a7b153f956b6f5091289e02146a
// IndexVersion: 1
// --- END CODE INDEX META ---
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
