using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IMQTTAppClient : IMQTTClientBase
    {
        String AppId { get; set; }
        String Password { get; set; }
        
        UInt16 SubscribeToApplicationStatus();

        UInt16 Subscribe(String topic, byte qosLevel = 0);
        UInt16 Publish<T>(String topic, T payload, byte qosLevel = 0);
        UInt16 Publish(String topic, String payload = "", byte qosLevel = 0);


        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();
        Task SaveSettingsAsync();
    }

}
