using System.Collections.Generic;
using LagoVista.Core.Models;

namespace LagoVista.Core.AI.Models
{
    public class AgentExecuteRequest
    {
        public EntityHeader AgentContext { get; set; }

        public EntityHeader ConversationContext { get; set; }

        public string ConversationId { get; set; }
        public string ResponseContinuationId { get; set; }

        public string Mode { get; set; }

        public string Instruction { get; set; }

        public string WorkspaceId { get; set; }

        public string Repo { get; set; }

        public string Language { get; set; }

        public string RagScope { get; set; }

        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        public List<ActiveFile> ActiveFiles { get; set; } = new List<ActiveFile>();
    }
}
