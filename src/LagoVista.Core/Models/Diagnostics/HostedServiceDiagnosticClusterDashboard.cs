using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models.Diagnostics
{
    public class HostedServiceDiagnosticClusterDashboard
    {
        public DateTime GeneratedUtc { get; set; }
        public string EnvironmentName { get; set; }
        public int InstanceCount { get; set; }
        public List<HostedServiceDiagnosticInstanceResult> Instances { get; set; } = new List<HostedServiceDiagnosticInstanceResult>();
    }
}