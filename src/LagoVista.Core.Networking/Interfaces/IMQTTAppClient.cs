using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IMQTTAppClient : IMQTTClientBase
    {
        String AppId { get; set; }
        String Password { get; set; }

        UInt16 Subscribe(string topic, QOS qosLevel = QOS.QOS0);
        UInt16 Publish<T>(string topic, T payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        UInt16 Publish(string topic, string payload = "", QOS qosLevel = QOS.QOS0, bool retainFlag = false);
        UInt16 Publish(string topic, byte[] payload, QOS qosLevel = QOS.QOS0, bool retainFlag = false);

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();
    }
}
