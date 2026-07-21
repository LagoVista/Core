using LagoVista.Core.Models;
using LagoVista.Core.Models.AIMetaData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.AI.Models
{
    public static class GuidedSopInteractionOperations
    {
        public const string SetValue = "set_value";
        public const string RemoveValue = "remove_value";
        public const string CompleteStage = "complete_stage";

        public static bool IsSupported(string operation)
        {
            return operation == SetValue || operation == RemoveValue || operation == CompleteStage;
        }
    }

    /// <summary>
    /// Optional structured input supplied with a normal agent turn while a Guided SOP is active.
    /// Instruction may be omitted when this payload fully expresses the user's action.
    /// </summary>
    public sealed class GuidedSopInteractionRequest
    {
        [JsonProperty("workItemId")]
        public string WorkItemId { get; set; }

        [JsonProperty("stageKey")]
        public string StageKey { get; set; }

        [JsonProperty("interactionId")]
        public string InteractionId { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }

        [JsonProperty("workingDataPath")]
        public string WorkingDataPath { get; set; }

        [JsonProperty("value")]
        public JToken Value { get; set; }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(WorkItemId))
                throw new InvalidOperationException("GuidedSopInteraction.WorkItemId is required.");

            if (String.IsNullOrWhiteSpace(StageKey))
                throw new InvalidOperationException("GuidedSopInteraction.StageKey is required.");

            if (String.IsNullOrWhiteSpace(Operation) || !GuidedSopInteractionOperations.IsSupported(Operation))
                throw new InvalidOperationException($"GuidedSopInteraction.Operation must be one of: {GuidedSopInteractionOperations.SetValue}, {GuidedSopInteractionOperations.RemoveValue}, {GuidedSopInteractionOperations.CompleteStage}.");

            if ((Operation == GuidedSopInteractionOperations.SetValue || Operation == GuidedSopInteractionOperations.RemoveValue) && String.IsNullOrWhiteSpace(WorkingDataPath))
                throw new InvalidOperationException("GuidedSopInteraction.WorkingDataPath is required for set_value and remove_value operations.");

            if (Operation == GuidedSopInteractionOperations.SetValue && Value == null)
                throw new InvalidOperationException("GuidedSopInteraction.Value is required for set_value operations.");
        }
    }

    /// <summary>
    /// User-facing projection of the active Guided SOP execution.
    /// Authoritative runtime state remains owned by the SOP execution domain.
    /// </summary>
    public sealed class GuidedSopExecutionWorkspace
    {
        [JsonProperty("vtmMeetingId")]
        public string VtmMeetingId { get; set; }

        [JsonProperty("workItemId")]
        public string WorkItemId { get; set; }

        [JsonProperty("standardOperatingProcedure")]
        public EntityHeader StandardOperatingProcedure { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("currentStage")]
        public GuidedSopStageWorkspace CurrentStage { get; set; }

        [JsonProperty("stages")]
        public List<GuidedSopStageSummary> Stages { get; set; } = new List<GuidedSopStageSummary>();

        [JsonProperty("workingData")]
        public JObject WorkingData { get; set; } = new JObject();

        [JsonProperty("requiredInputs")]
        public List<GuidedSopRequiredInput> RequiredInputs { get; set; } = new List<GuidedSopRequiredInput>();

        [JsonProperty("canCompleteStage")]
        public bool CanCompleteStage { get; set; }
    }

    public sealed class GuidedSopStageWorkspace
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("interactionId")]
        public string InteractionId { get; set; }

        [JsonProperty("fields")]
        public List<GuidedSopFieldWorkspace> Fields { get; set; } = new List<GuidedSopFieldWorkspace>();
    }

    public sealed class GuidedSopFieldWorkspace
    {
        [JsonProperty("workingDataPath")]
        public string WorkingDataPath { get; set; }

        [JsonProperty("field")]
        public AiFieldDescriptor Field { get; set; }

        [JsonProperty("value")]
        public JToken Value { get; set; }

        [JsonProperty("isSatisfied")]
        public bool IsSatisfied { get; set; }

        [JsonProperty("requiresHumanConfirmation")]
        public bool RequiresHumanConfirmation { get; set; }
    }

    public sealed class GuidedSopStageSummary
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Transport projection of a SopWorkItemInput. This intentionally avoids coupling Core to AI runtime models.
    /// </summary>
    public sealed class GuidedSopRequiredInput
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("requiredBySopStep")]
        public string RequiredBySopStep { get; set; }

        [JsonProperty("missingInputPrompt")]
        public string MissingInputPrompt { get; set; }

        [JsonProperty("requiresHumanInput")]
        public bool RequiresHumanInput { get; set; }

        [JsonProperty("artifact")]
        public EntityHeader Artifact { get; set; }

        [JsonProperty("producedInMeeting")]
        public EntityHeader ProducedInMeeting { get; set; }
    }
}
