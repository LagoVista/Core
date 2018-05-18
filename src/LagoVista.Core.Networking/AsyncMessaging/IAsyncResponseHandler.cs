using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncResponseHandler
    {
        Task HandleResponse(IAsyncResponse response);
    }
}
