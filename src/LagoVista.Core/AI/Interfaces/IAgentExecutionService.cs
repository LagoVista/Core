using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.AI.Models;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;

namespace LagoVista.Core.AI.Interfaces
{
    public interface IAgentExecutionService
    {
        Task<InvokeResult<AgentExecuteResponse>> ExecuteAsync(AgentExecuteRequest request,
            EntityHeader org, EntityHeader user, CancellationToken cancellationToken = default);
    }
}
