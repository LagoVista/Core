using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RemoteControlWelcome
    {
        public string SessionId { get; set; }
        public string StreamUrl { get; set; }
        public string SessionToken { get; set; }
        public DateTimeOffset ExpiresUtc { get; set; }
        public int HeartbeatIntervalSeconds { get; set; }
        public int HeartbeatTimeoutSeconds { get; set; }
        public int MaxPayloadBytes { get; set; }
        public string AcceptedProtocolVersion { get; set; }

        public RemoteControlWelcome()
        {
            SessionId = String.Empty;
            StreamUrl = String.Empty;
            SessionToken = String.Empty;
            AcceptedProtocolVersion = String.Empty;
        }
    }
}