using LagoVista.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.Tests.Utils
{
    /// <summary>
    /// fake sender "sends" a request, then waits a bit to simulate network and then completes the request with the coupler
    /// </summary>
    internal sealed class FakeSender : IAsyncRequestHandler
    {
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly object _result;


        public FakeSender(IAsyncCoupler<IAsyncResponse> asyncCoupler, object result)
        {
            _asyncCoupler = asyncCoupler;
            _result = result;
        }

        public Task HandleRequest(IAsyncRequest request)
        {
            CompleteRequest(request, TimeSpan.FromSeconds(1));
            return Task.FromResult<object>(null);
        }

        private void CompleteRequest(IAsyncRequest request, TimeSpan delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(delay);
                var response = new AsyncResponse(request, _result);
                await _asyncCoupler.CompleteAsync(response.CorrelationId, response);
            });
        }
    }

}
