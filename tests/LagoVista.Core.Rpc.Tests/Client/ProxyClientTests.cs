using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Tests.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Client
{
    [TestClass]
    public class ProxyClientTests
    {
        public ITransceiver ProxyClient_Constructor()
        {
            var client = new SimulatedProxyClient(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                new Mock<ILogger>().Object,
                Constants.QueueSimulator);

            return client;
        }

        [TestMethod]
        public async Task ProxyClient_TransmitAsync()
        {
            var client = ProxyClient_Constructor();
            var request = RequestTests.CreateControlEchoRequest();
            var requestCount = Constants.QueueSimulator.RequestCount;
            await client.TransmitAsync(request);
            Assert.IsTrue(Constants.QueueSimulator.RequestCount > requestCount);
        }

        [TestMethod]
        public async Task ProxyClient_ReceiveAsync_InvokeResult_Success()
        {
            var client = ProxyClient_Constructor();
            var request = RequestTests.CreateControlEchoRequest();
            await client.TransmitAsync(request);
            CompleteRequest(client, request, TimeSpan.FromSeconds(1));
            var invokeResult = await Constants.AsyncCoupler.WaitOnAsync(request.CorrelationId, TimeSpan.FromSeconds(5));
            Assert.IsTrue(invokeResult.Successful);
        }

        [TestMethod]
        public async Task ProxyClient_ReceiveAsync_InvokeResult_Result_IsCorrectType()
        {
            var client = ProxyClient_Constructor();
            var request = RequestTests.CreateControlEchoRequest();
            await client.TransmitAsync(request);
            CompleteRequest(client, request, TimeSpan.FromSeconds(1));
            var invokeResult = await Constants.AsyncCoupler.WaitOnAsync(request.CorrelationId, TimeSpan.FromSeconds(5));
            Assert.IsInstanceOfType(invokeResult.Result, typeof(IResponse));
        }

        [TestMethod]
        public async Task ProxyClient_ReceiveAsync_Response_Success()
        {
            var client = ProxyClient_Constructor();
            var request = RequestTests.CreateControlEchoRequest();
            await client.TransmitAsync(request);
            CompleteRequest(client, request, TimeSpan.FromSeconds(1));
            var invokeResult = await Constants.AsyncCoupler.WaitOnAsync(request.CorrelationId, TimeSpan.FromSeconds(5));
            var response = (IResponse)invokeResult.Result;
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ProxyClient_ReceiveAsync_Response_ExpectedValues()
        {
            var client = ProxyClient_Constructor();
            var request = RequestTests.CreateControlEchoRequest();
            await client.TransmitAsync(request);
            CompleteRequest(client, request, TimeSpan.FromSeconds(1));
            var invokeResult = await Constants.AsyncCoupler.WaitOnAsync(request.CorrelationId, TimeSpan.FromSeconds(5));
            var response = (IResponse)invokeResult.Result;
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.ReturnValue);
        }

        private void CompleteRequest(ITransceiver client, IRequest request, TimeSpan delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(delay);
                var response = new Response(request, ProxySubject.EchoValueConst);
                await client.ReceiveAsync(response);
            });
        }
    }
}
