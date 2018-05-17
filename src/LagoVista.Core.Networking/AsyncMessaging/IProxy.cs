using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IProxy
    {
        Task<IAsyncResponse> CallRemoteAsync(IAsyncRequest message);
    }

    public interface IProxy<TInterface>
    {

    }
}
