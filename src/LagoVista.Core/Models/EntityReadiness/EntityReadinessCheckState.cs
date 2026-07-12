using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.EntityReadiness
{
    public sealed class EntityReadinessCheckState
    {
        public EntityReadinessCheckType CheckType { get; set; }

        public EntityReadinessCheckStatus Status { get; set; }

        public int RequiredStepCount { get; set; }

        public int CompletedStepCount { get; set; }

        public string Summary { get; set; }

        public List<string> BlockingReasons { get; set; } = new List<string>();

        public UtcTimestamp? EvaluatedUtc { get; set; }
    }

    public enum EntityReadinessCheckType
    {
        CoreDefinition,
        CatalogDefinition,
        EntityDefinition,
        RelationshipReadiness,
        OperationalReadiness
    }

    public enum EntityReadinessCheckStatus
    {
        NotEvaluated,
        Ready,
        NeedsAttention,
        Blocked,
        Failed
    }
}
