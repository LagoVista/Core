using LagoVista.Core.Rpc.Messages;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface IReceiver
    {
        Task StartAsync();        

        Task ReceiveAsync(IMessage message);
    }
}
