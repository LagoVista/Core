using System.Collections.Generic;

namespace LagoVista.Core.AI.Models
{
    public class AgentExecuteResponse
    {
        public string Kind { get; set; }

        public string ConversationId { get; set; }
        public string TurnId { get; set; }
        public string AgentContextId { get; set; }

        public string ConversationContextId { get; set; }
        public string ResponseContinuationId { get; set; }

        public string RequestEnvelopeUrl { get; set; }
        public string FullResponseUrl { get; set; }

        public string Mode { get; set; }

        public List<SourceRef> Sources { get; set; } = new List<SourceRef>();

        public FileBundle FileBundle { get; set; }

        public List<string> Warnings { get; set; } = new List<string>();

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>
        /// The model identifier actually used for this turn (e.g., gpt-5.1).
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// Aggregated natural language text extracted from the LLM output
        /// (if any). Typically the concatenation of all text segments.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Why the model stopped generating (e.g., stop, length, tool_use, error).
        /// </summary>
        public string FinishReason { get; set; }

        /// <summary>
        /// Token usage reported by the LLM, if available.
        /// </summary>
        public LlmUsage Usage { get; set; } = new LlmUsage();

        /// <summary>
        /// Raw JSON payload from the underlying /responses call.
        /// This allows the client (VS Code extension) to inspect tool calls directly.
        /// </summary>
        public string RawResponseJson { get; set; }

        /// <summary>
        /// Parsed high-level tool calls (optional convenience).
        /// For v1 you can treat this as opaque, or just rely on RawResponseJson.
        /// </summary>
        public List<AgentToolCall> ToolCalls { get; set; } = new List<AgentToolCall>();
    }

    public class AgentToolCall
    {
        public string CallId { get; set; }
        public string Name { get; set; }
        public string ArgumentsJson { get; set; }

        // New: where did we expect this tool to run?
        public bool IsServerTool { get; set; }        // true = server, false = client
        public bool WasExecuted { get; set; }         // did *anyone* run it yet?

        // New: tool execution result (if available)
        public string ResultJson { get; set; }        // raw JSON result payload
        public string ErrorMessage { get; set; }      // error from server-side execution, if any
    }


    public class LlmUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
