using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    public enum ProcessInstanceStatus
    {
        Running,
        Paused,
        Completed
    }

    public class ProcessDesignerLayout
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class ProcessGuard
    {
        public string Key { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class ProcessActivity
    {
        public string Id { get; set; }
        public string ActivityType { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }

    public class ProcessState
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsTerminal { get; set; }
        public ProcessDesignerLayout Layout { get; set; } = new ProcessDesignerLayout();
        public List<ProcessActivity> EntryActivities { get; set; } = new List<ProcessActivity>();
        public List<ProcessActivity> ExitActivities { get; set; } = new List<ProcessActivity>();
    }

    public class ProcessTransition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SourceStateId { get; set; }
        public string TargetStateId { get; set; }
        public List<ProcessGuard> Guards { get; set; } = new List<ProcessGuard>();
        public List<ProcessActivity> Activities { get; set; } = new List<ProcessActivity>();
        public ProcessDesignerLayout Layout { get; set; } = new ProcessDesignerLayout();
    }

    public class ProcessDefinition : EntityBase
    {
        public int Revision { get; set; } = 1;
        public string InitialStateId { get; set; }
        public List<ProcessState> States { get; set; } = new List<ProcessState>();
        public List<ProcessTransition> Transitions { get; set; } = new List<ProcessTransition>();
    }

    public class ProcessInstance : EntityBase
    {
        public string DefinitionId { get; set; }
        public int DefinitionRevision { get; set; }
        public string CurrentStateId { get; set; }
        public ProcessInstanceStatus Status { get; set; } = ProcessInstanceStatus.Running;
        public string StartedAtUtc { get; set; }
        public string UpdatedAtUtc { get; set; }
    }
}
