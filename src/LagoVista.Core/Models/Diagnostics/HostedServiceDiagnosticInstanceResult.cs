namespace LagoVista.Core.Models.Diagnostics
{
    public class HostedServiceDiagnosticInstanceResult
    {
        public string PodName { get; set; }
        public string PodIp { get; set; }
        public string Module { get; set; }
        public string Tier { get; set; }
        public string NodeName { get; set; }
        public bool IsCurrentPod { get; set; }
        public bool Successful { get; set; }
        public string Error { get; set; }
        public HostedServiceDiagnosticDashboard Dashboard { get; set; }
    }
}