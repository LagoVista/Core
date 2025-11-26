using System.Collections.Generic;

namespace LagoVista.Core.AI.Models
{
    public class AgentExecuteResponse
    {
        public string Kind { get; set; }

        public string ConversationId { get; set; }

        public string AgentContextId { get; set; }

        public string ConversationContextId { get; set; }
        public string ResponseContinuationId { get; set; }

        public string Mode { get; set; }

        public string Text { get; set; }

        public List<SourceRef> Sources { get; set; } = new List<SourceRef>();

        public FileBundle FileBundle { get; set; }

        public List<string> Warnings { get; set; } = new List<string>();

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
