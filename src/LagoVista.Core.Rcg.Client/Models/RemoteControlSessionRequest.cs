using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RemoteControlSessionRequest
    {
        public string TargetId { get; set; }
        public string TargetInstanceId { get; set; }
        public string OrganizationId { get; set; }
        public string TargetType { get; set; }
        public string ProtocolVersion { get; set; }

        public RemoteControlSessionRequest()
        {
            TargetId = String.Empty;
            TargetInstanceId = String.Empty;
            OrganizationId = String.Empty;
            TargetType = String.Empty;
            ProtocolVersion = "1.0";
        }
    }
}
