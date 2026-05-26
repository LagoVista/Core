using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public enum ArtifactAuthoringStatus
    {
        NotStarted,
        GatheringContext,
        NeedsClarification,
        Ready,
        Committing,
        Committed,
        Blocked,
        Failed
    }

    public enum ArtifactAuthoringCommitIntent
    {
        None,
        Create,
        Update,
        Regenerate,
        Revise
    }

    public class ArtifactAuthoringState
    {
        public EntityHeader ArtifactSpecification { get; set; }

        public EntityHeader TargetArtifact { get; set; }

        public bool RootArtifactOnly { get; set; } = true;

        public EntityHeader<ArtifactAuthoringStatus> Status { get; set; } =
            EntityHeader<ArtifactAuthoringStatus>.Create(ArtifactAuthoringStatus.NotStarted);

        public EntityHeader<ArtifactAuthoringCommitIntent> LastCommitIntent { get; set; } =
            EntityHeader<ArtifactAuthoringCommitIntent>.Create(ArtifactAuthoringCommitIntent.None);

        public double? ContextCompletenessScore { get; set; }

        public double? ReadinessScore { get; set; }

        public double? ArtifactQualityScore { get; set; }

        public string ActiveContractKfrKey { get; set; }

        public string ReadinessKfrKey { get; set; }

        public string LastPreviewArtifactId { get; set; }

        public string CurrentGoalSummary { get; set; }

        public string MissingContextSummary { get; set; }

        public string QualitySummary { get; set; }

        public string NextBestQuestion { get; set; }

        public string LastCommitUtc { get; set; }

        public string LastUpdatedUtc { get; set; } = UtcTimestamp.Now.Value;
    }
}
