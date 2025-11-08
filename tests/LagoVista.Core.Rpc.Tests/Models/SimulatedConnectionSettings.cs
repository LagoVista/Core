// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c6ce166a05f342ddd5062ff3f30c2069f6c3cf3b4f14f6b8c91fa3bef29cea09
// IndexVersion: 2
// --- END CODE INDEX META ---
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
