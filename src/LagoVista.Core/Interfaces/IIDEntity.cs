using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Interfaces
{
    public interface IIDEntity
    {
        NormalizedId32 Id { get; set; }
    }

    public interface IRelationalIDEntity
    {
        GuidString36 Id { get; set; }
    }

    public interface ICheckListEntity
    {
        List<EntityChecklistStatus> ChecklistStatus { get;  }
    }

    public interface ICoreDefinitionEntity : ICheckListEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        string Purpose { get; set; }
        string PurposeSummary { get; set; }

        bool IsCoreReady { get; set; }
        int CoreDefinitionScore { get; set; }
        UtcTimestamp? CoreDefinitionReviewedUtc { get; set; }
        string CoreDefinitionSummary { get; set; }
    }

    public interface IEntityDefinitionReadiness : ICoreDefinitionEntity
    {
        bool IsDefinitionReady { get; set; }

        UtcTimestamp? DefinitionReadyUtc { get; set; }

        IReadOnlyList<EntityChecklistStageDefinition> GetDefinitionReadinessStages();
    }

    public class EntityChecklistStageDefinition
    {
        public EntityChecklistStageDefinition()
        {
            RequiredCompletedStepKeys = new List<string>();
            CompletedStepKeys = new List<string>();
            TargetIncompleteStepKeys = new List<string>();
        }

        public string StageKey { get; set; }

        public string StageName { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Optional guidance describing the objective of this stage and how its
        /// checklist steps should be approached together. This augments the
        /// guidance defined on each individual checklist step.
        /// </summary>
        public string Guidance { get; set; }

        /// <summary>
        /// Checklist steps that must be completed before this stage is eligible
        /// to run.
        /// </summary>
        public List<string> RequiredCompletedStepKeys { get; set; }

        /// <summary>
        /// Checklist steps that must be completed for this stage to be considered
        /// complete.
        /// </summary>
        public List<string> CompletedStepKeys { get; set; }

        /// <summary>
        /// Checklist steps this stage should improve when they are incomplete.
        /// Steps are processed in the order they are declared.
        /// </summary>
        public List<string> TargetIncompleteStepKeys { get; set; }

    }

    public static class EntityDefinitionReadinessEvaluator
    {
        public static bool Recalculate(IEntityDefinitionReadiness entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var stages = entity.GetDefinitionReadinessStages() ??
                         new List<EntityChecklistStageDefinition>();

            var completedStepKeys = new HashSet<string>(
                (entity.ChecklistStatus ?? new List<EntityChecklistStatus>())
                    .Where(status =>
                        status != null &&
                        status.Status?.Value == EntityChecklistStepStatus.Completed &&
                        !String.IsNullOrWhiteSpace(status.StepKey))
                    .Select(status => status.StepKey),
                StringComparer.OrdinalIgnoreCase);

            var requiredStepKeys = stages
                .Where(stage => stage != null)
                .SelectMany(stage => stage.CompletedStepKeys ?? new List<string>())
                .Where(stepKey => !String.IsNullOrWhiteSpace(stepKey))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var wasReady = entity.IsDefinitionReady;

            entity.IsDefinitionReady =
                entity.IsCoreReady &&
                requiredStepKeys.Count > 0 &&
                requiredStepKeys.All(completedStepKeys.Contains);

            if (entity.IsDefinitionReady && !wasReady)
            {
                entity.DefinitionReadyUtc = UtcTimestamp.Now;
            }
            else if (!entity.IsDefinitionReady)
            {
                entity.DefinitionReadyUtc = null;
            }

            return wasReady != entity.IsDefinitionReady;
        }
    }
}
