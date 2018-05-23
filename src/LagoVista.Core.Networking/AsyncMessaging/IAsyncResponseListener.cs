using LagoVista.Core.PlatformSupport;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncResponseListener
    {
        void Start(IAsyncResponseHandler responseHandler, IListenerConnectionSettings connectionSettings, ILogger logger);
    }
}
