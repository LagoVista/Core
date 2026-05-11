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

}
