using LagoVista.Core.Models;
using LagoVista.Core.Models.EntityReadiness;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IEntityReadinessDefinition : IFormCheckListSteps
    {
        string EntityType { get; set; }

        IReadOnlyCollection<EntityReadinessCheckType> GetSupportedReadinessChecks();

        IReadOnlyCollection<EntityChecklistStepReference> GetRequiredChecklistSteps(EntityReadinessCheckType checkType);

        List<EntityReadinessCheckState> ReadinessChecks { get; set; }

        List<EntityChecklistStatus> ChecklistStatus { get; set; }
    }
}
