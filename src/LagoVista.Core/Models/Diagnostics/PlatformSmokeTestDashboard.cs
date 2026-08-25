using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Models.Diagnostics
{
    public class PlatformSmokeTestDashboard
    {
        public string InstanceName { get; set; }
        public string EnvironmentName { get; set; }
        public DateTime GeneratedUtc { get; set; }
        public List<PlatformSmokeTestResult> Tests { get; set; } = new List<PlatformSmokeTestResult>();

        public int PassedCount => Tests.Count(test => test.Status == PlatformSmokeTestStatus.Passed);
        public int FailedCount => Tests.Count(test => test.Status == PlatformSmokeTestStatus.Failed);
        public int SkippedCount => Tests.Count(test => test.Status == PlatformSmokeTestStatus.Skipped);
        public bool Successful => FailedCount == 0;
    }
}
