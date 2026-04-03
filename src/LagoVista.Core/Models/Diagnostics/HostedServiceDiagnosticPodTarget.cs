using System.Collections.Generic;

namespace LagoVista.Core.Models.Diagnostics
{
    public class HostedServiceDiagnosticPodTarget
    {
        public string PodName { get; set; }
        public string PodIp { get; set; }
        public string Namespace { get; set; }
        public string Module { get; set; }
        public string Tier { get; set; }
        public string NodeName { get; set; }
        public bool IsCurrentPod { get; set; }
        public Dictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
    }
}