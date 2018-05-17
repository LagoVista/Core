using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public interface ISender
    {
        Task SendAsync(IMessage message);
    }
}
