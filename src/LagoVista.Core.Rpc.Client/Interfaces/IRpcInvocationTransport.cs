using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client.Interfaces
{
    public interface IRpcInvocationTransport
    {
        Task<InvokeResult<IResponse>> InvokeAsync(IRequest request, TimeSpan timeout);
    }
}
