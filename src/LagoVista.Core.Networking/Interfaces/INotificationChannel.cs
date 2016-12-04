using System;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface INotificationChannel
    {
        String Id { get; set; }
        String DeviceId { get; set; }
        String ChannelType { get; set; }
        String ChannelURI { get; set; }
        String ChannelRegistrationTimeStamp { get; set; }
        String Active { get; set; }
        String LastPush { get; set; }
        String LastError { get; set; }
        String LastErrorTimeStamp { get; set; }
    }
}
