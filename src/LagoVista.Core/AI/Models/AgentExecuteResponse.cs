using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LagoVista.Core.AI.Models
{
    /// <summary>
    /// AGN-033 — Model-agnostic response contract for agent execution APIs.
    ///
    /// Notes:
    /// - This payload is typically wrapped by InvokeResult&lt;T&gt;. If invocation is not successful,
    ///   AgentExecuteResponseV2 is expected to be null/empty and errors live in InvokeResult.
    /// - Diagnostics and vendor/raw LLM payloads are intentionally out-of-band.
    /// </summary>
    public sealed class AgentExecuteResponse
    {
        /// <summary>
        /// Closed enum discriminator for AGN-033.
        /// Allowed values: final, client_tool_continuation.
        /// </summary>
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AgentExecuteResponseKind Kind { get; set; }

        /// <summary>
        /// Identifies the session (the “room”) in which this turn exists.
        /// Always present.
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// Identifies the turn within the session. TurnId is only valid within its SessionId room.
        /// Always present.
        /// </summary>
        [JsonProperty("turnId")]
        public string TurnId { get; set; }

        /// <summary>
        /// Display-only mode label. Clients MUST NOT branch logic based on this value.
        /// Always present.
        /// </summary>
        [JsonProperty("modeDisplayName")]
        public string ModeDisplayName { get; set; }

        /// <summary>
        /// Final-only. Exactly one PrimaryOutputText MUST be present when Kind == final.
        /// Canonical format: Markdown text (UTF-8 string).
        /// </summary>
        [JsonProperty("primaryOutputText", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryOutputText { get; set; }

        /// <summary>
        /// Tool-continuation-only. Optional informational message the client may display.
        /// Must never require user action.
        /// </summary>
        [JsonProperty("toolContinuationMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ToolContinuationMessage { get; set; }

        /// <summary>
        /// Tool-continuation-only. One or more ToolCalls MUST be present when Kind == client_tool_continuation.
        /// ToolCalls must never appear in final responses.
        /// </summary>
        [JsonProperty("toolCalls", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientToolCall> ToolCalls { get; set; }

        /// <summary>
        /// Final-only. Optional tool results for visibility into server-executed tools.
        /// ToolResults MUST NOT appear in tool-continuation responses.
        /// </summary>
        [JsonProperty("toolResults", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientToolResult> ToolResults { get; set; }

        /// <summary>
        /// Final-only. Optional supplemental file references associated with the final response.
        /// Files MUST NOT appear in tool-continuation responses.
        /// </summary>
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public List<FileRef> Files { get; set; }

        /// <summary>
        /// Final-only. Non-fatal, user-facing informational warnings.
        /// </summary>
        [JsonProperty("userWarnings", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> UserWarnings { get; set; }

        /// <summary>
        /// Final-only. Token usage for the completed turn.
        /// </summary>
        [JsonProperty("usage", NullValueHandling = NullValueHandling.Ignore)]
        public LlmUsage Usage { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AgentExecuteResponseKind
    {
        [EnumMember(Value = "final")]
        Final,

        [EnumMember(Value = "client_tool_continuation")]
        ClientToolContinuation
    }

    /// <summary>
    /// Tool call request emitted by the server for client execution.
    /// </summary>
    public sealed class ClientToolCall
    {
        [JsonProperty("toolCallId")]
        public string ToolCallId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Raw JSON string for tool arguments.
        /// </summary>
        [JsonProperty("argumentsJson")]
        public string ArgumentsJson { get; set; }
    }

    /// <summary>
    /// Tool execution result returned by client (or surfaced by server for visibility).
    /// Tool results for client-tool flows must match the exact ordered ToolCall set.
    /// </summary>
    public sealed class ClientToolResult
    {
        [JsonProperty("toolCallId")]
        public string ToolCallId { get; set; }

        [JsonProperty("executionMs")]
        public int ExecutionMs { get; set; }

        /// <summary>
        /// Exactly one of ResultJson or ErrorMessage should be present.
        /// </summary>
        [JsonProperty("resultJson", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultJson { get; set; }

        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// File reference contract (no embedded file contents).
    /// ContentHash is computed by the platform-standard IContentHashService (IDX-016).
    /// ContentExpires is ISO-8601 if present.
    /// </summary>
    public sealed class FileRef
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("sizeBytes")]
        public long SizeBytes { get; set; }

        [JsonProperty("contentHash")]
        public string ContentHash { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("contentExpires", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentExpires { get; set; }
    }

    public sealed class LlmUsage
    {
        [JsonProperty("promptTokens")]
        public int PromptTokens { get; set; }

        [JsonProperty("completionTokens")]
        public int CompletionTokens { get; set; }

        [JsonProperty("totalTokens")]
        public int TotalTokens { get; set; }
    }
}
