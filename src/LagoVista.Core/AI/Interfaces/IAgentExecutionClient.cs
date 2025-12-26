using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.AI.Models;
using LagoVista.Core.Models;

namespace LagoVista.Core.AI.Interfaces
{
    public interface IAgentExecutionClient
    {
        Task<AgentExecuteResponse> ExecuteAsync(AgentExecuteRequest request,
            CancellationToken cancellationToken = default);

        Task<AgentExecuteResponse> AskAsync(EntityHeader agentContext, EntityHeader conversationContext,
            string instruction, string conversationId = null, string workspaceId = null,
            string repo = null, string language = null, string ragScope = null,
            IEnumerable<InputArtifact> inputArtifacts = null,
            CancellationToken cancellationToken = default);

        Task<AgentExecuteResponse> EditAsync(EntityHeader agentContext, EntityHeader conversationContext,
            string instruction, IEnumerable<InputArtifact> inputArtifacts, string conversationId = null,
            string workspaceId = null, string repo = null, string language = null,
            string ragScope = null, CancellationToken cancellationToken = default);
    }
}
