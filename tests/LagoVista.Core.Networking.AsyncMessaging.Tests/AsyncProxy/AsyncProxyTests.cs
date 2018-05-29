using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Validation;
using LagoVista.Core.Networking.AsyncMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncProxyTests
{
    [TestClass]
    public class AsyncProxyTests : AsyncProxyFactoryTests
    {
        private readonly IAsyncRequest controlEchoRequest = CreateControlEchoRequest();
        private readonly Exception responseFailException = new Exception(rootExceptionValue, new Exception("hoo"));
        private IAsyncResponse controlEchoSuccessResponse = null;
        private IAsyncResponse controlEchoFailureResponse = null;
        private readonly TimeSpan timeSpan = TimeSpan.FromMinutes(1);

        [TestInitialize]
        public void Init()
        {
            controlEchoSuccessResponse = new AsyncResponse(controlEchoRequest, ProxySubject.EchoValueConst);
            successCoupler.Setup(proc => proc.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
                Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(controlEchoSuccessResponse)));

            controlEchoFailureResponse = new AsyncResponse(controlEchoRequest, responseFailException);
            failCoupler.Setup(proc => proc.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
                Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(controlEchoFailureResponse)));

            //_couplerSuccess.Setup(proc => proc.CompleteAsync(It.IsAny<string>(), It.IsAny<IAsyncResponse>())).Returns(Task.FromResult(InvokeResult.Success));

            sender.Setup(proc => proc.HandleRequest(It.IsAny<IAsyncRequest>())).Returns(Task.FromResult<object>(null));
        }

        [TestMethod]
        public async Task AsyncProxy_CallSimpleMethod_ResultIsExpected()
        {
            IProxySubject proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, sender.Object);
            var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);

            Assert.IsNotNull(echoResult);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public async Task AsyncProxy_CallSimpleMethod_CouplerSuccessMoqVerified()
        {
            IProxySubject proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, sender.Object);
            var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);

            var correlationIdArg = It.Is<string>(s => string.Compare(s, controlEchoSuccessResponse.CorrelationId) == 0);
            var timeSpanArg = It.Is<TimeSpan>(t => t.Ticks == this.timeSpan.Ticks);
            successCoupler.Verify(o => o.WaitOnAsync(correlationIdArg, timeSpanArg));
        }

        [TestMethod]
        public async Task AsyncProxy_CallSimpleMethod_SenderMoqVerified()
        {
            IProxySubject proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, sender.Object);
            var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);

            sender.Verify(o => o.HandleRequest(It.Is<IAsyncRequest>(arc => arc.Id == controlEchoRequest.Id)));
        }

        [TestMethod]
        public async Task AsyncProxy_CallSimpleMethod_FailureResponse()
        {
            IProxySubject proxy = proxyFactory.Create<IProxySubject>(failCoupler.Object, sender.Object);
            var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);

            Assert.IsNotNull(echoResult);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public async Task AsyncProxy_CallSimpleMethod_FailureResponse_CouplerSuccessMoqVerified()
        {
            IProxySubject proxy = proxyFactory.Create<IProxySubject>(failCoupler.Object, sender.Object);
            var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);

            var correlationIdArg = It.Is<string>(s => string.Compare(s, controlEchoSuccessResponse.CorrelationId) == 0);
            var timeSpanArg = It.Is<TimeSpan>(t => t.Ticks == this.timeSpan.Ticks);
            failCoupler.Verify(o => o.WaitOnAsync(correlationIdArg, timeSpanArg));
        }

        [TestMethod]
        public async Task AsyncProxy_CallSimpleMethod_FailureResponse_SenderMoqVerified()
        {
            IProxySubject proxy = proxyFactory.Create<IProxySubject>(failCoupler.Object, sender.Object);
            var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);

            sender.Verify(o => o.HandleRequest(It.Is<IAsyncRequest>(arc => arc.Id == controlEchoRequest.Id)));
        }
    }
}
