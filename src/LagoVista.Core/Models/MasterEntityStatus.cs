using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public sealed class MasterEntityStatus
    {
        public bool IsBlocked { get; set; }

        public List<EntityBlockingWorkItem> BlockingWorkItems { get; set; } = new List<EntityBlockingWorkItem>();

        public string NextChecklistStepKey { get; set; }
        public bool IsProductionReady { get; set; }

        public UtcTimestamp? ProductionReadyDate { get; set; }
    }

    public sealed class EntityBlockingWorkItem
    {
        public EntityHeader WorkItem { get; set; }

        public UtcTimestamp BlockedUtc { get; set; }
    }
}
