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
        Task<InvokeResult> ListenAsync(RcgRuntimeChannelConnection connection, IRcgRuntimeCommandHandler handler, CancellationToken cancellationToken);
        Task<InvokeResult> SendFrameAsync(RcgRuntimeChannelConnection connection, RemoteControlFrame frame, CancellationToken cancellationToken);
    }
}
