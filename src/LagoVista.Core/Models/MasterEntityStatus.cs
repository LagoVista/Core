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

        public string NextImprovementStageKey { get; set; }

        public string ImprovementSpecificationVersion { get; set; }

        public UtcTimestamp? ImprovementStatusReconciledUtc { get; set; }

        /// <summary>Non-blocking snapshot of the latest complete quality assessment.
        /// Full criterion evidence and history live in GradingAssessmentRecord.</summary>
        public decimal? QualityScore { get; set; }
        public string QualityGrade { get; set; }
        public string GradingSpecificationVersion { get; set; }
        /// <summary>NotGraded, Partial, Graded or Stale; never controls production readiness.</summary>
        public string GradingStatus { get; set; }
        public UtcTimestamp? LastGradedUtc { get; set; }
        /// <summary>Authoritative evaluated source fingerprint, for freshness reconciliation.</summary>
        public string GradingSourceFingerprint { get; set; }

        public string NextReadinessStage { get; set; }
        public bool IsProductionReady { get; set; }
        public int TotalChecklistCount { get; set; }
        public int CompletedChecklistCount { get; set; }
        public int TotalReadinessCheckCount { get; set; }
        public int CompletedReadinessCheckCount { get; set; }
        public UtcTimestamp? ProductionReadyDate { get; set; }
        public int FailedChecklistCount { get; set; }

        public string LastFailedChecklistStepKey { get; set; }

        public UtcTimestamp? LastChecklistFailureUtc { get; set; }

        public string LastChecklistFailureSummary { get; set; }
    }

    public sealed class EntityBlockingWorkItem
    {
        public EntityHeader WorkItem { get; set; }

        public UtcTimestamp BlockedUtc { get; set; }
    }
}
