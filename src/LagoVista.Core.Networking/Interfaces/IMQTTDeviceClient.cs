using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IMQTTDeviceClient : IMQTTClientBase
    {
        String DeviceId { get; set; }
        String Password { get; set; }

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();

        UInt16 Subscribe(String topic, byte qosLevel = 0);
        UInt16 Publish<T>(String topic, T payload, byte qosLevel = 0);
        UInt16 Publish(String topic, String payload = "", byte qosLevel = 0);
    }

}
