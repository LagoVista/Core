using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncProxyFactory
    {
        TProxy Create<TProxy>(IAsyncCoupler<IAsyncResponse> asyncCoupler, IAsyncRequestHandler requestSender);
    }
}
