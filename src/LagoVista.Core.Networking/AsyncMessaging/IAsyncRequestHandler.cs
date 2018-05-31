using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    //todo: ML - add <summary> for this interface
    public interface IAsyncRequestHandler
    {
        Task HandleRequest(IAsyncRequest request, string instructions = null);
    }
}
