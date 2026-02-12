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

        [JsonProperty("clientAcpCalls", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientAcpCall> ClientAcpCalls { get; set; }

        [JsonProperty("clientAcpCallResults", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientAcpCallResult> ClientAcpCallResults { get; set; }

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

        [JsonProperty("totalSessionTokens", NullValueHandling = NullValueHandling.Ignore)]
        public long TotalSessionTokens { get; set; }

        /// <summary>
        /// Error Code for Call if Present
        /// </summary>
        [JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Error Message for call if present
        /// </summary>
        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }


        [JsonProperty("acpIntents", NullValueHandling = NullValueHandling.Ignore)]
        public List<AcpIntent> AcpIntents { get; set; } = new List<AcpIntent>();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AgentExecuteResponseKind
    {
        [EnumMember(Value = "final")]
        Final,

        [EnumMember(Value = "client_tool_continuation")]
        ClientToolContinuation,


        [EnumMember(Value = "error")]
        Error
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

        [JsonProperty("kind")]
        public string Kind { get; set; }
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
   
    public sealed class ClientAcpCall
    {
        [JsonProperty("interactionId")]
        public string InteractionId { get; set; }

        [JsonProperty("commandId")]
        public string CommandId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }   // e.g. "picker", "confirm" or "acp.command_picker"

        [JsonProperty("argumentsJson")]
        public string ArgumentsJson { get; set; } // JSON payload for UI rendering
    }

    public sealed class ClientAcpCallResult
    {
        [JsonProperty("interactionId")]
        public string InteractionId { get; set; }

        [JsonProperty("commandId")]
        public string CommandId { get; set; }

        [JsonProperty("executionMs")]
        public int ExecutionMs { get; set; }    

        [JsonProperty("resultJson")]
        public string ResultJson { get; set; } // JSON payload of selection/confirmation/etc.

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

        [JsonProperty("cachedTokens")]
        public int CachedTokends { get; set; }

        [JsonProperty("totalTokens")]
        public int TotalTokens { get; set; }

        [JsonProperty("reasoningTokens")]
        public int ReasoningTokens { get; set; }
   }


    public interface IUIIntentPayload
    {

    }

    public sealed class AcpIntent
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UiIntentKind Kind { get; set; }

        // Correlation id so the client can post back a structured selection later
        [JsonProperty("intentId")]
        public string IntentId { get; set; }

        // Display hints
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        // Generic payload so you can evolve without breaking schema
        [JsonProperty("payload", NullValueHandling = NullValueHandling.Ignore)]
        public IUIIntentPayload Payload { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UiIntentKind
    {
        [EnumMember(Value = "picker")]
        Picker,

        [EnumMember(Value = "confirm")]
        Confirm,

        [EnumMember(Value = "notification")]
        Notification
    }

    public sealed class PickerIntentPayload : IUIIntentPayload
    {
        [JsonProperty("prefilterText", NullValueHandling = NullValueHandling.Ignore)]
        public string PrefilterText { get; set; }

        [JsonProperty("allowSearch")]
        public bool AllowSearch { get; set; } = true;

        [JsonProperty("items")]
        public List<PickerItem> Items { get; set; } = new List<PickerItem>();
    }

    public sealed class PickerItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }

    public sealed class ConfirmIntentPayload : IUIIntentPayload
    {
        [JsonProperty("confirmText")]
        public string ConfirmText { get; set; }

        [JsonProperty("confirmLabel", NullValueHandling = NullValueHandling.Ignore)]
        public string ConfirmLabel { get; set; } = "Yes";

        [JsonProperty("cancelLabel", NullValueHandling = NullValueHandling.Ignore)]
        public string CancelLabel { get; set; } = "No";
    }

    public sealed class NotificationIntentPayload : IUIIntentPayload
    {
        [JsonProperty("level")]
        public string Level { get; set; } // "info" | "warning" | "error"

        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public string Details { get; set; }
    }
}
