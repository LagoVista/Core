using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface IReceiver
    {
        Task<InvokeResult> ReceiveAsync(IMessage message);
    }
}
