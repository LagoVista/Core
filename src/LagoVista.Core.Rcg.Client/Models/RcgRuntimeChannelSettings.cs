using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgRuntimeChannelSettings
    {
        public string GatewayBaseUrl { get; set; }
        public string RuntimeKey { get; set; }
        public string TargetId { get; set; }
        public string TargetInstanceId { get; set; }
        public string TargetType { get; set; }
        public string OrganizationId { get; set; }
        public string UserId { get; set; }
        public string ProtocolVersion { get; set; }
        public int TimeoutSeconds { get; set; }

        public RcgRuntimeChannelSettings()
        {
            GatewayBaseUrl = String.Empty;
            RuntimeKey = String.Empty;
            TargetId = String.Empty;
            TargetInstanceId = String.Empty;
            TargetType = "runtime";
            OrganizationId = String.Empty;
            UserId = String.Empty;
            ProtocolVersion = "1.0";
            TimeoutSeconds = 30;
        }
    }
}
