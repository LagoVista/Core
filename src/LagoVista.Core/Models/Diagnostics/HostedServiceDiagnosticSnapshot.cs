using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.Diagnostics
{
    public enum HostedServiceDiagnosticStatus
    {
        Starting,
        Running,
        Error,
        Stopped
    }

    public class HostedServiceDiagnosticLogEntry
    {
        public DateTime TimestampUtc { get; set; }
        public string Message { get; set; }
    }


    public class HostedServiceDiagnosticSnapshot
    {
        public string Name { get; set; }
        public HostedServiceDiagnosticStatus Status { get; set; }
        public DateTime? StartedUtc { get; set; }
        public DateTime? LastActivityUtc { get; set; }
        public string LastActivity { get; set; }
        public DateTime? LastErrorUtc { get; set; }
        public string LastError { get; set; }
        public int ExpectedActivityWindowSeconds { get; set; }

        public List<HostedServiceDiagnosticLogEntry> RecentEntries { get; set; } = new List<HostedServiceDiagnosticLogEntry>();
    }
}
