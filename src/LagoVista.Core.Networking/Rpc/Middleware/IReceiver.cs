using LagoVista.Core.Networking.Rpc.Messages;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.Middleware
{
    public interface IReceiver
    {
        void Start();

        Task ReceiveAsync(IMessage message);
    }
}
