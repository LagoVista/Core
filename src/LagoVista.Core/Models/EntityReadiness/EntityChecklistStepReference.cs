using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.EntityReadiness
{
    public sealed class EntityChecklistStepReference
    {
        public EntityChecklistStepType StepType { get; set; }

        public string TargetName { get; set; }

        public static EntityChecklistStepReference Review(string targetName)
        {
            return Create(EntityChecklistStepType.Review, targetName);
        }

        public static EntityChecklistStepReference Improve(string targetName)
        {
            return Create(EntityChecklistStepType.Improve, targetName);
        }

        public static EntityChecklistStepReference Validate(string targetName)
        {
            return Create(EntityChecklistStepType.Validate, targetName);
        }

        public static EntityChecklistStepReference Create(string targetName)
        {
            return Create(EntityChecklistStepType.Create, targetName);
        }

        private static EntityChecklistStepReference Create(EntityChecklistStepType stepType, string targetName)
        {
            if (String.IsNullOrWhiteSpace(targetName))
            {
                throw new ArgumentNullException(nameof(targetName));
            }

            return new EntityChecklistStepReference
            {
                StepType = stepType,
                TargetName = targetName
            };
        }
    }
}
