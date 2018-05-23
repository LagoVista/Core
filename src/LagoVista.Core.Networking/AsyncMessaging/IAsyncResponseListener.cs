using LagoVista.Core.PlatformSupport;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncResponseListener
    {
        //IAsyncResponseHandler responseHandler, IListenerConnectionSettings connectionSettings, ILogger logger
        void Start();
    }
}
