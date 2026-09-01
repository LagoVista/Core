using System;
using System.Collections.Generic;

namespace LagoVista.Core.Configuration
{
    public class RemoteConfigurationResponse
    {
        public RemoteConfigurationResponse()
        {
            Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string AppKey { get; set; }

        public string DeploymentKey { get; set; }

        public DateTime GeneratedDateUtc { get; set; }

        public Dictionary<string, string> Values { get; set; }
    }
}
