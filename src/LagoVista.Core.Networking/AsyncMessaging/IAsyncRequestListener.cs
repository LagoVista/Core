using LagoVista.Core.PlatformSupport;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequestListener
    {
        void Start(IAsyncRequestBroker requestBroker, IListenerConnectionSettings connectionSettings, ILogger logger);
    }
}
