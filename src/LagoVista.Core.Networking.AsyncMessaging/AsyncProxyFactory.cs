using LagoVista.Core.Interfaces;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class AsyncProxyFactory : IAsyncProxyFactory
    {
        public TProxy Create<TProxy>(IAsyncCoupler<IAsyncResponse> asyncCoupler, IAsyncRequestHandler sender)
        {
            var result = DispatchProxy.Create<TProxy, AsyncProxy>();
            (result as AsyncProxy).AsyncCoupler = asyncCoupler;
            (result as AsyncProxy).RequestSender = sender;
            return result;
        }
    }
}
