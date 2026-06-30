using System;

namespace LagoVista.Core.Models.EntityReadiness
{
    public sealed class EntityChecklistStepReference
    {
        public EntityChecklistStepType StepType { get; set; }

        public string TargetName { get; set; }

        public string StepKey { get; set; }

        public static EntityChecklistStepReference Review(string targetName)
        {
            return Create(EntityChecklistStepType.Review, targetName, null);
        }

        public static EntityChecklistStepReference Review(string targetName, string stepKey)
        {
            return Create(EntityChecklistStepType.Review, targetName, stepKey);
        }

        public static EntityChecklistStepReference Improve(string targetName)
        {
            return Create(EntityChecklistStepType.Improve, targetName, null);
        }

        public static EntityChecklistStepReference Improve(string targetName, string stepKey)
        {
            return Create(EntityChecklistStepType.Improve, targetName, stepKey);
        }

        public static EntityChecklistStepReference Validate(string targetName)
        {
            return Create(EntityChecklistStepType.Validate, targetName, null);
        }

        public static EntityChecklistStepReference Validate(string targetName, string stepKey)
        {
            return Create(EntityChecklistStepType.Validate, targetName, stepKey);
        }

        public static EntityChecklistStepReference Create(string targetName)
        {
            return Create(EntityChecklistStepType.Create, targetName, null);
        }

        public static EntityChecklistStepReference Create(string targetName, string stepKey)
        {
            return Create(EntityChecklistStepType.Create, targetName, stepKey);
        }

        public string GetResolvedStepKey()
        {
            if (!String.IsNullOrWhiteSpace(StepKey))
                return StepKey;

            if (String.IsNullOrWhiteSpace(TargetName))
                throw new InvalidOperationException("A checklist step reference must have a target name.");

            var normalizedTargetName = Char.ToLowerInvariant(TargetName[0]) + TargetName.Substring(1);
            var stepTypeHeader = EntityHeader<EntityChecklistStepType>.Create(StepType);

            return $"{normalizedTargetName}.{stepTypeHeader.Id}";
        }

        private static EntityChecklistStepReference Create(EntityChecklistStepType stepType, string targetName, string stepKey)
        {
            if (String.IsNullOrWhiteSpace(targetName))
            {
                throw new ArgumentNullException(nameof(targetName));
            }

            return new EntityChecklistStepReference
            {
                StepType = stepType,
                TargetName = targetName,
                StepKey = stepKey
            };
        }
    }
}