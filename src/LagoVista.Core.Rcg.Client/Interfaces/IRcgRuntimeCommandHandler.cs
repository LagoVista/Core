using LagoVista.Core.Rcg.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgRuntimeCommandHandler
    {
        Task<RemoteControlFrame> HandleAsync(RcgRuntimeCommandContext context, CancellationToken cancellationToken);
    }
}
