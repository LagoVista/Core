using System;

namespace LagoVista.Core.Networking.Interfaces
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

}
