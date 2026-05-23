using System;
using System.Collections.Generic;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RemoteControlDiagnosticsSnapshot
    {
        public DateTimeOffset CreatedUtc { get; set; }
        public int ConnectedTargetCount { get; set; }
        public int PendingSessionCount { get; set; }
        public List<RemoteControlConnectionSummary> Connections { get; set; }

        public RemoteControlDiagnosticsSnapshot()
        {
            Connections = new List<RemoteControlConnectionSummary>();
        }
    }
}
