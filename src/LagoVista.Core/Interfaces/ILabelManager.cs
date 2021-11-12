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
        Task<ListResponse<ILabeledEntity>> GetLabeledEntitiesAsync(string labelId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
