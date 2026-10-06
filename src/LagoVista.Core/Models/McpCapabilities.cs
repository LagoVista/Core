using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    [Flags]
    public enum McpToolUsage
    {
        None = 0,
        ModelVisible = 1,
        DeterministicTransition = 2,
        Both = ModelVisible | DeterministicTransition
    }

    public class McpServerConnection
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }

    public class McpToolCapability
    {
        public string ServerId { get; set; }
        public string ToolName { get; set; }
        public string Description { get; set; }
        public string InputSchemaJson { get; set; }
        public string ContractHash { get; set; }
    }

    public class McpServerCapabilitySnapshot
    {
        public string ServerId { get; set; }
        public string ServerName { get; set; }
        public string Endpoint { get; set; }
        public string DiscoveredAtUtc { get; set; }
        public List<McpToolCapability> Tools { get; set; } = new List<McpToolCapability>();
    }

    public class McpToolBinding
    {
        public string ServerId { get; set; }
        public string ToolName { get; set; }
        public string ToolContractHash { get; set; }
        public McpToolUsage Usage { get; set; }
        public Dictionary<string, string> InputMappings { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, object> StaticInputs { get; set; } = new Dictionary<string, object>();
    }

    public class McpToolInvocationRequest
    {
        public McpToolBinding Binding { get; set; }
        public Dictionary<string, object> ProcessContext { get; set; } = new Dictionary<string, object>();
    }

    public class McpToolInvocationResult
    {
        public bool Succeeded { get; set; }
        public string ServerId { get; set; }
        public string ToolName { get; set; }
        public string StartedAtUtc { get; set; }
        public string CompletedAtUtc { get; set; }
        public long DurationMilliseconds { get; set; }
        public string ResultJson { get; set; }
        public string Error { get; set; }
        public string ObservedContractHash { get; set; }
    }
}
