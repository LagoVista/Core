using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IMQTTAppClient : IMQTTClientBase
    {
        String AppId { get; set; }
        String Password { get; set; }

        Task<UInt16> SubscribeAsync(string topic, QOS qosLevel = QOS.QOS0);
        Task<UInt16> PublishAsync<T>(string topic, T payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, string payload = "", QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        Task<UInt16> PublishAsync(string topic, byte[] payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();
    }
}
