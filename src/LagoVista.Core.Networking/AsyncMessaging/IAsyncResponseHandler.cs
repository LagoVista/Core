using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    //todo: ML - add <summary> for this interface
    public interface IAsyncResponseHandler
    {
        Task HandleResponse(IAsyncResponse response);
    }
}
