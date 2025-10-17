// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8297f24f525ca5342c4c0177ba5641865da8d5e1771a54ae1e8ab9df5538dce3
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Networking.Resources;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public enum QOS
    {
        [EnumLabel("qos0", NetworkingResources.Names.MQTT_QOS0, typeof(NetworkingResources))]
        QOS0,
        [EnumLabel("qos1", NetworkingResources.Names.MQTT_QOS1, typeof(NetworkingResources))]
        QOS1,
        [EnumLabel("qos2", NetworkingResources.Names.MQTT_QOS2, typeof(NetworkingResources))]
        QOS2
    }

    public interface IMQTTDeviceClient : IMQTTClientBase
    {
        String DeviceId { get; set; }
        String Password { get; set; }

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();

        Task<UInt16> SubscribeAsync(MQTTSubscription subscription);
        Task<UInt16> PublishAsync<T>(string topic, T payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, string payload = "", QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, byte[] payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
    }
}
