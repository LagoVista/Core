using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgRuntimeChannelClient
    {
        Task<InvokeResult<RemoteControlWelcome>> RequestChannelAsync(EntityHeader org, EntityHeader user);
        Task<InvokeResult<RcgRuntimeChannelConnection>> ConnectAsync(EntityHeader org, EntityHeader user, CancellationToken cancellationToken);
    }
}
