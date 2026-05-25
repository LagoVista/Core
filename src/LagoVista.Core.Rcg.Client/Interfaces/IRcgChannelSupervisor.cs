using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgChannelSupervisor
    {
        Task<InvokeResult> RunAsync(EntityHeader org, EntityHeader user, IRcgRuntimeCommandHandler handler, CancellationToken cancellationToken);
    }
}
