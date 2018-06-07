using LagoVista.Core.Networking.Rpc.Messages;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.Middleware
{
    public interface ITransmitter
    {
        Task<IMessage> TransmitAsync(IMessage message);
    }
}
