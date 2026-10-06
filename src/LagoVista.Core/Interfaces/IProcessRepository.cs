using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IProcessDefinitionRepository
    {
        Task AddAsync(ProcessDefinition definition);
        Task<ProcessDefinition> GetAsync(string id);
        Task UpdateAsync(ProcessDefinition definition);
    }

    public interface IProcessInstanceRepository
    {
        Task AddAsync(ProcessInstance instance);
        Task<ProcessInstance> GetAsync(string id);
        Task UpdateAsync(ProcessInstance instance);
    }
}
