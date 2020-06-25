using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface ITransmitter
    {
        Task<InvokeResult> TransmitAsync(IMessage message);
    }
}
