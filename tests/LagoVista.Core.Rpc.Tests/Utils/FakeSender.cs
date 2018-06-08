//using LagoVista.Core.Interfaces;
//using LagoVista.Core.Rpc.Messages;
//using System;
//using System.Threading.Tasks;

//namespace LagoVista.Core.Rpc.Tests.Utils
//{
//    /// <summary>
//    /// fake sender "sends" a request, then waits a bit to simulate network and then completes the request with the coupler
//    /// </summary>
//    internal sealed class FakeSender : IRequestHandler
//    {
//        private readonly IAsyncCoupler<IResponse> _asyncCoupler;
//        private readonly object _result;


//        public FakeSender(IAsyncCoupler<IResponse> asyncCoupler, object result)
//        {
//            _asyncCoupler = asyncCoupler;
//            _result = result;
//        }

//        public Task HandleRequest(IRequest request)
//        {
//            CompleteRequest(request, TimeSpan.FromSeconds(1));
//            return Task.FromResult<object>(null);
//        }

//        private void CompleteRequest(IRequest request, TimeSpan delay)
//        {
//            Task.Factory.StartNew(async () =>
//            {
//                await Task.Delay(delay);
//                var response = new Response(request, _result);
//                await _asyncCoupler.CompleteAsync(response.CorrelationId, response);
//            });
//        }
//    }

//}
