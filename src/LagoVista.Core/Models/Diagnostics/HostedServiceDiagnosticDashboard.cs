using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.Diagnostics
{
    public class HostedServiceDiagnosticDashboard
    {
        public string InstanceName { get; set; }
        public string EnvironmentName { get; set; }
        public string Version { get; set; }
        public DateTime GeneratedUtc { get; set; }
        public List<HostedServiceDiagnosticSnapshot> Services { get; set; } = new List<HostedServiceDiagnosticSnapshot>();
    
    
    }
}
