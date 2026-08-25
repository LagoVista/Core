using System;

namespace LagoVista.Core.Models.Diagnostics
{
    public enum PlatformSmokeTestStatus
    {
        Passed,
        Failed,
        Skipped
    }

    public class PlatformSmokeTestResult
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public PlatformSmokeTestStatus Status { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
        public long DurationMs { get; set; }
        public DateTime CheckedUtc { get; set; }
    }
}
