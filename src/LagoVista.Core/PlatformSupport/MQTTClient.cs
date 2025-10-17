// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7c5c841647d86a5f19b6cd9528a309615d6feb7c456f2c700776ae2378755585
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public enum ConnAck
    {
        Accepted = 0x00,
        RefusedProtocolVersion = 0x01,
        IndentifierRejected = 0x02,
        ServerUnavailable = 0x03,
        BadUserNamePassword = 0x04,
        NotAuthorized = 0x05,
        Exception = 0x98,
        TimeOut = 0x99,
    }

    public interface IMQTTAppStatusReceivedEventArgs
    {
        String AppId { get; }
        String Payload { get; }

        T DeserializePayload<T>();
    }

    public interface IMQTTEventDeviceStatusReceivedEventArgs
    {
        String DeviceType { get; }
        String DeviceId { get; }
        String Payload { get; }

        T DeserializePayload<T>();
    }

    public interface IMQTTCommandEventArgs
    {
        String CommandName { get; }
        String Format { get; }

        String Payload { get; }

        T DeserializePayload<T>();
    }

    public interface IMQTTEventReceivedEventArgs
    {
        String DeviceType { get; }
        String DeviceId { get; }
        String EventName { get; }
        String Format { get; }
        String Payload { get; }

        T DeserializePayload<T>();
    }


    public interface IMQTTIoTClientBase
    {
        event EventHandler<IMQTTAppStatusReceivedEventArgs> AppStatusReceived;
        event EventHandler<IMQTTCommandEventArgs> CommandReceived;
        event EventHandler<IMQTTEventReceivedEventArgs> EventReceived;
        event EventHandler<IMQTTEventDeviceStatusReceivedEventArgs> DeviceStatusReceived;

        event EventHandler<bool> ConnectionStateChanged;

        String OrgId { get; set; }        
        String APIToken { get; set; }

        String ClientId { get; }

        bool IsConnected { get; }

        Task<ConnAck> Connect();

        bool ShowDiagnostics { get; set; }
    }


    public interface IMQTTIoTAppClient : IMQTTIoTClientBase
    {
        String AppId { get; set; }
        String APIKey { get; set; }

        String ServerURL { get; set; }
        UInt16 SubscribeToApplicationStatus();

        UInt16 SubscribeToDeviceEvents(string deviceType = "+", string deviceId = "+", string evt = "+", string format = "+");

        UInt16 SubscribeToDeviceCommands(string deviceType = "+", string deviceId = "+", string cmd = "+", string format = "+");

        UInt16 PublishCommand(String deviceType, String deviceId, String command, string format, string data);

        UInt16 PublishEvent(String deviceType, String deviceId, String evt, string format, string data);
        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();

        Task SaveSettingsAsync();
    }

    public interface IMQTTIoTDeviceClient : IMQTTIoTClientBase
    {
        String DeviceType { get; set; }

        String DeviceId { get; set; }
        String ServerURL { get; set; }

        bool SettingsReady { get; }

        Task<bool> ReadSettingsAsync();

        Task SaveSettingsAsync();

        UInt16 SubscribeCommand(String cmd, String format, byte qosLevel = 0);

        UInt16 PublishEvent<T>(String evt, String format, T payload);

        UInt16 PublishEvent(String evt, String format, String msg, byte qosLevel = 0);
    }

}
