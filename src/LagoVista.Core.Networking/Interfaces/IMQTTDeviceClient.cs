using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public enum QOS
    {
        QOS0,
        QOS1,
        QOS2
    }

    public interface IMQTTDeviceClient : IMQTTClientBase
    {
        String DeviceId { get; set; }
        String Password { get; set; }

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();

        Task<UInt16> SubscribeAsync(string topic, QOS qosLevel = QOS.QOS0);
        Task<UInt16> PublishAsync<T>(string topic, T payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, string payload = "", QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, byte[] payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
    }
}
