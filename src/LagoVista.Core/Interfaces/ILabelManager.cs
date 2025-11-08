// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4ad62a086178c6b07a64059617d67e2c9b6caaef2a2c322404bd09d0aa5aab5a
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ILabelManager
    {
        Task<InvokeResult<LabelSet>> GetLabelSetAsync(EntityHeader org, EntityHeader user);
        Task<InvokeResult<LabelSet>> AddLabelAsync(Label label, EntityHeader org, EntityHeader user);
        Task<InvokeResult<LabelSet>> UpdateLabelAsync(Label label, EntityHeader org, EntityHeader user);
        Task<ListResponse<LabeledEntity>> GetLabeledEntitiesAsync(string labelId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
