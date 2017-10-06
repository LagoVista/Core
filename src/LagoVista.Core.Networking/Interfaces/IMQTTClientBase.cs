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