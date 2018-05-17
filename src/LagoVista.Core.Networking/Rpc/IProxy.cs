using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public interface IProxy
    {
        Task<IResponse> CallRemoteAsync(IRequest message);
    }
}
