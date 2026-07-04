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

    public interface ICoreDefinitionEntity : ICheckListEntity, IOwnershipPoints
    {
        string Name { get; set; }
        string Description { get; set; }
        string Purpose { get; set; }
        string PurposeSummary { get; set; }

    }

    public interface IEntityDefinitionReadiness : ICoreDefinitionEntity
    {
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
}
