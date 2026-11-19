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
        /// <summary>Diagnostic quality acceptance, independent of GradingStatus,
        /// improvement completion and IsProductionReady.</summary>
        public string MasterQualityStatus { get; set; }
        /// <summary>Version of the separately managed quality acceptance policy.</summary>
        public string QualityStandardVersion { get; set; }
        public string GradingSpecificationVersion { get; set; }
        /// <summary>NotGraded, Partial, Graded or Stale; never controls production readiness.</summary>
        public string GradingStatus { get; set; }
        public UtcTimestamp? LastGradedUtc { get; set; }
        /// <summary>Authoritative evaluated source fingerprint, for freshness reconciliation.</summary>
        // Retained only for backward compatibility with earlier assessments.
        // No longer a grading prerequisite or a gate.
        public string GradingSourceFingerprint { get; set; }
        /// <summary>Compact field/criterion grade projection. Detailed findings and
        /// evidence are stored only in GradingAssessmentRecord.</summary>
        public List<MasterEntityQualityCriterion> QualityCriteria { get; set; } =
            new List<MasterEntityQualityCriterion>();

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

    /// <summary>Three non-blocking master-quality determinations.
    /// A score is never changed to make a standard pass or fail.</summary>
    public static class MasterQualityStates
    {
        public const string MeetsStandard = "MeetsStandard";
        public const string NeedsImprovement = "NeedsImprovement";
        public const string NotYetEvaluated = "NotYetEvaluated";
    }

    public sealed class MasterEntityQualityCriterion
    {
        public string CriterionKey { get; set; }
        public decimal? Score { get; set; }
        public int GradedFields { get; set; }
        public int TotalFields { get; set; }
        public List<MasterEntityQualityField> Fields { get; set; } = new List<MasterEntityQualityField>();
    }

    public sealed class MasterEntityQualityField
    {
        public string FieldKey { get; set; }
        public decimal? Score { get; set; }
    }

    public sealed class EntityBlockingWorkItem
    {
        public EntityHeader WorkItem { get; set; }

        public UtcTimestamp BlockedUtc { get; set; }
    }
}
