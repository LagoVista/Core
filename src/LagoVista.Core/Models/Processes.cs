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

        /// <summary>
        /// Optional MCP binding for activities backed by a discovered tool capability.
        /// The binding retains the discovered contract hash so designers/runtime can
        /// detect server-side schema changes before execution.
        /// </summary>
        public McpToolBinding McpBinding { get; set; }
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
        public string InitialStateId { get; set; }
        public List<ProcessState> States { get; set; } = new List<ProcessState>();
        public List<ProcessTransition> Transitions { get; set; } = new List<ProcessTransition>();
    }

    public class ProcessRuntimeHistoryEntry
    {
        public string EventType { get; set; }
        public string TimestampUtc { get; set; }
        public string TransitionId { get; set; }
        public string SourceStateId { get; set; }
        public string TargetStateId { get; set; }
        public ProcessInstanceStatus Status { get; set; }
        public string Message { get; set; }
        public string Failure { get; set; }
    }

    public class ProcessActivityExecutionHistoryEntry
    {
        public string ActivityId { get; set; }
        public string ActivityType { get; set; }
        public string Name { get; set; }
        public string TransitionId { get; set; }
        public string StateId { get; set; }
        public string Status { get; set; }
        public string StartedAtUtc { get; set; }
        public string CompletedAtUtc { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public long ModelActiveMilliseconds { get; set; }
        public long ToolCommandMilliseconds { get; set; }
        public int RetryCount { get; set; }
        public int FailureCount { get; set; }
        public int CallCount { get; set; }
        public string Failure { get; set; }
    }

    public class ProcessRuntimeTelemetry
    {
        public long ElapsedMilliseconds { get; set; }
        public long ModelActiveMilliseconds { get; set; }
        public long ToolCommandMilliseconds { get; set; }
        public long WaitingMilliseconds { get; set; }
        public int RetryCount { get; set; }
        public int FailureCount { get; set; }
        public int CallCount { get; set; }
    }

    public class ProcessContinuationContext
    {
        public string CurrentStateId { get; set; }
        public ProcessInstanceStatus Status { get; set; } = ProcessInstanceStatus.Running;
        public string LastTransitionId { get; set; }
        public string PausedAtUtc { get; set; }
        public string LastUpdatedAtUtc { get; set; }
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }

    public class ProcessInstance : EntityBase
    {
        public string DefinitionId { get; set; }
        public int DefinitionRevision { get; set; }
        public string CurrentStateId { get; set; }
        public ProcessInstanceStatus Status { get; set; } = ProcessInstanceStatus.Running;
        public string StartedAtUtc { get; set; }
        public string UpdatedAtUtc { get; set; }

        public List<ProcessRuntimeHistoryEntry> RuntimeHistory { get; set; } = new List<ProcessRuntimeHistoryEntry>();
        public List<ProcessActivityExecutionHistoryEntry> ActivityHistory { get; set; } = new List<ProcessActivityExecutionHistoryEntry>();
        public ProcessRuntimeTelemetry Telemetry { get; set; } = new ProcessRuntimeTelemetry();
        public ProcessContinuationContext Continuation { get; set; } = new ProcessContinuationContext();
    }
}
