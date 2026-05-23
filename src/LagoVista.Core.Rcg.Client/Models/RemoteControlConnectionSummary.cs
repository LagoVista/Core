using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RemoteControlConnectionSummary
    {
        public string SessionId { get; set; }
        public string TargetId { get; set; }
        public string TargetInstanceId { get; set; }
        public string OrganizationId { get; set; }
        public string ProtocolVersion { get; set; }
        public DateTimeOffset ConnectedUtc { get; set; }
        public DateTimeOffset LastActivityUtc { get; set; }
        public int PendingRequestCount { get; set; }
        public string Status { get; set; }

        public RemoteControlConnectionSummary()
        {
            SessionId = String.Empty;
            TargetId = String.Empty;
            TargetInstanceId = String.Empty;
            OrganizationId = String.Empty;
            ProtocolVersion = String.Empty;
            Status = String.Empty;
        }
    }
}
