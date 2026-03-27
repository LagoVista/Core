using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models.Configuration
{
    public class ResolvedConfiguration
    {
        public ResolvedConfiguration()
        {
            Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            GeneratedDateUtc = DateTime.UtcNow;
        }

        public string AppKey { get; set; }

        public string DeploymentKey { get; set; }

        public DateTime GeneratedDateUtc { get; set; }

        public Dictionary<string, string> Values { get; set; }
    }
}
