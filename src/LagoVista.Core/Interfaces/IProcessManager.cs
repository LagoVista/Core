using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IProcessManager
    {
        Task<ProcessDefinition> CreateDefinitionAsync(ProcessDefinition definition);
        Task<ProcessDefinition> GetDefinitionAsync(string id);
        Task<ProcessDefinition> UpdateDefinitionAsync(ProcessDefinition definition);
        Task<ProcessInstance> StartAsync(string definitionId);
        Task<ProcessInstance> GetInstanceAsync(string id);
        Task<ProcessInstance> TransitionAsync(string instanceId, string transitionId);
        Task<ProcessInstance> PauseAsync(string instanceId);
        Task<ProcessInstance> ResumeAsync(string instanceId);
    }
}
