using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgStatusManager
    {
        Task<InvokeResult<RemoteControlDiagnosticsSnapshot>> GetDiagnosticsAsync(EntityHeader org, EntityHeader user);
    }
}
