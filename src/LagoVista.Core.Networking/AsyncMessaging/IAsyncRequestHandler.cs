using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequestHandler
    {
        Task HandleRequest(IAsyncRequest request);
    }
}
