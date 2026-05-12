using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{

    public enum EntityChecklistStepStatus
    {
        [EnumLabel(EntityChecklistStatus.NotStarted, LagoVistaCommonStrings.Names.EntityChecklistStepStatus_NotStarted, typeof(LagoVistaCommonStrings))]
        NotStarted,

        [EnumLabel(EntityChecklistStatus.InProgress, LagoVistaCommonStrings.Names.EntityChecklistStepStatus_InProgress, typeof(LagoVistaCommonStrings))]
        InProgressLagoVistaCommonStrings,

        [EnumLabel(EntityChecklistStatus.NeedsAttention, LagoVistaCommonStrings.Names.EntityChecklistStepStatus_NeedsAttention, typeof(LagoVistaCommonStrings))]
        NeedsAttention,

        [EnumLabel(EntityChecklistStatus.Completed, LagoVistaCommonStrings.Names.EntityChecklistStepStatus_Completed, typeof(LagoVistaCommonStrings))]
        Completed,
    }

    public class EntityChecklistStatus
    {
        public const string NotStarted = "notstarted";
        public const string InProgress = "inprogress";
        public const string NeedsAttention = "needsattention";
        public const string Completed = "completed";

        public EntityChecklistStatus()
        {
            Status = EntityHeader<EntityChecklistStepStatus>.Create(EntityChecklistStepStatus.NotStarted);
        }

        public string StepKey { get; set; }

        public string StepName { get; set; }

        public EntityHeader<EntityChecklistStepStatus> Status { get; set; }

        public UtcTimestamp? TimeStamp { get; set; }

        public EntityHeader UpdatedBy { get; set; }

        public string Notes { get; set; }
    }

    public enum EntityChecklistStepType
    {
        [EnumLabel(EntityChecklistStep.StepType_Review, LagoVistaCommonStrings.Names.EntityChecklistStepType_Review, typeof(LagoVistaCommonStrings))]
        Review,

        [EnumLabel(EntityChecklistStep.StepType_Improve, LagoVistaCommonStrings.Names.EntityChecklistStepType_Improve, typeof(LagoVistaCommonStrings))]
        Improve,

        [EnumLabel(EntityChecklistStep.StepType_Create, LagoVistaCommonStrings.Names.EntityChecklistStepType_Create, typeof(LagoVistaCommonStrings))]
        Create,

        [EnumLabel(EntityChecklistStep.StepType_Validate, LagoVistaCommonStrings.Names.EntityChecklistStepType_Validate, typeof(LagoVistaCommonStrings))]
        Validate,
    }


    public class EntityChecklistStep
    {
        public const string StepType_Review = "review";
        public const string StepType_Improve = "improve";
        public const string StepType_Create = "create";
        public const string StepType_Validate = "validate";
        public string Key { get; set; }

        public string FieldName { get; set; }

        public EntityHeader<EntityChecklistStepType> StepType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PromptActionLabel { get; set; }

        public string AiPrompt { get; set; }

        public static EntityChecklistStep Review(string fieldName, string name, string description, string promptActionLabel = null, string aiPrompt = null)
        {
            return ForField(fieldName, EntityChecklistStepType.Review, name, description, promptActionLabel, aiPrompt);
        }

        public static EntityChecklistStep Improve(string fieldName, string name, string description, string promptActionLabel = null, string aiPrompt = null)
        {
            return ForField(fieldName, EntityChecklistStepType.Improve, name, description, promptActionLabel, aiPrompt);
        }

        public static EntityChecklistStep Validate(string fieldName, string name, string description, string promptActionLabel = null, string aiPrompt = null)
        {
            return ForField(fieldName, EntityChecklistStepType.Validate, name, description, promptActionLabel, aiPrompt);
        }

        public static EntityChecklistStep Create(string key, string name, string description, string promptActionLabel = null, string aiPrompt = null)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return new EntityChecklistStep
            {
                Key = key,
                StepType = EntityHeader<EntityChecklistStepType>.Create(EntityChecklistStepType.Create),
                Name = name,
                Description = description,
                PromptActionLabel = promptActionLabel ?? $"Create {name}",
                AiPrompt = aiPrompt,
            };
        }

        public static EntityChecklistStep ForField(string fieldName, EntityChecklistStepType stepType, string name, string description, string promptActionLabel = null, string aiPrompt = null)
        {
            if (String.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            var normalizedFieldName = NormalizeFieldName(fieldName);
            var stepTypeHeader = EntityHeader<EntityChecklistStepType>.Create(stepType);

            return new EntityChecklistStep
            {
                Key = $"{normalizedFieldName}.{stepTypeHeader.Id}",
                FieldName = normalizedFieldName,
                StepType = stepTypeHeader,
                Name = name,
                Description = description,
                PromptActionLabel = promptActionLabel ?? BuildPromptActionLabel(stepTypeHeader.Text, name),
                AiPrompt = aiPrompt,
            };
        }

        private static string NormalizeFieldName(string fieldName)
        {
            if (String.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            return Char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
        }

        private static string BuildPromptActionLabel(string stepTypeText, string name)
        {
            if (String.IsNullOrWhiteSpace(stepTypeText))
            {
                throw new ArgumentNullException(nameof(stepTypeText));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return $"{stepTypeText} {name}";
        }
    }
}
