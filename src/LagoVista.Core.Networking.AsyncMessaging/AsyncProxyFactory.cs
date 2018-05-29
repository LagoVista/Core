using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class AsyncProxyFactory : IAsyncProxyFactory
    {
        public TProxy Create<TProxy>(IAsyncCoupler<IAsyncResponse> asyncCoupler, IAsyncRequestHandler requestSender)
        {
            return AsyncProxy.Create<TProxy>(asyncCoupler, requestSender);
        }
    }
}
