using LagoVista.Core.Rpc.Messages;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface ITransmitter
    {
        Task<IMessage> TransmitAsync(IMessage message);
    }
}
