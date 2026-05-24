using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgRpcClientTransport
    {
        Task<InvokeResult<RcgRpcClientTransportResponse>> SendAsync(RcgRpcClientTransportRequest request, EntityHeader org, EntityHeader user);
    }
}
