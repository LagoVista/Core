// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: febbfd5ca7c538292777fe8eb9041f80d690bc0793a6c8c162d65bcd8a3a7fa1
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Networking.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IMQTTClientBase : IDisposable
    {
        event EventHandler<MqttMsgPublishEventArgs> MessageReceived;
        event EventHandler<bool> ConnectionStateChanged;

        String BrokerHostName { get; set; }
        int BrokerPort { get; set; }

        String ClientId { get; }

        bool IsConnected { get; }

        Task<MQTTConnectResult> ConnectAsync(bool isSSL = false);

        void Disconnect();

        bool ShowDiagnostics { get; set; }
    }
}