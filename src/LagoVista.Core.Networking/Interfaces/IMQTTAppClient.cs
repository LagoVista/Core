// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 40ff871e1fbb4cc845c8fa7d0545aaffb9baf70d1e6f711bda8cd1721fadf621
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Networking.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IMQTTAppClient : IMQTTClientBase
    {
        String AppId { get; set; }
        String Password { get; set; }

        Task<UInt16> SubscribeAsync(MQTTSubscription subscription);
        Task<UInt16> PublishAsync<T>(string topic, T payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, string payload = "", QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, byte[] payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();
    }
}
