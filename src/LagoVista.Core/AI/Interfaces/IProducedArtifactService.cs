using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.AI.Interfaces
{
    public interface IProducedArtifactService
    {
        Task<InvokeResult> CreateProducedArtifactsAsync(IEntityBase entity);
    }
}
