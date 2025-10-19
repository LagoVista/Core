// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b753ad6eee632a5f8aa6f35c475d1c34af7c4d96746c7d7f299ce659b34a18d4
// IndexVersion: 0
// --- END CODE INDEX META ---
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
