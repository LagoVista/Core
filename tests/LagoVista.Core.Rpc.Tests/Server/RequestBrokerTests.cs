using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Tests.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class RequestBrokerTests
    {
        [TestMethod]
        public void RequestBroker_RegisterSubject_Success()
        {
            IProxySubject controlInstance = new ProxySubject();

            var broker = new RequestBroker();
            var methodsRegistered = broker.AddService(controlInstance);

            var instanceMethodCount = typeof(IProxySubject).GetMethods().Count();
            Assert.AreEqual(instanceMethodCount - 1, methodsRegistered);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RequestBroker_RegisterSubject_FailureDueToNonInterfaceInstance()
        {
            var broker = new RequestBroker();
            broker.AddService(new ProxySubject());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequestBroker_RegisterSubject_FailureDueToNullInstance()
        {
            var broker = new RequestBroker();
            broker.AddService((IProxySubject)null);
        }

        [TestMethod]
        public async Task RequestBroker_Invoke()
        {
            IProxySubject controlInstance = new ProxySubject();

            var broker = new RequestBroker();
            var methodsRegistered = broker.AddService(controlInstance);
            var request = RequestTests.CreateControlEchoRequest();

            var response = await broker.InvokeAsync(request);

            Assert.AreEqual(true, response.Success);
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(request.Id, response.RequestId);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.ReturnValue);
        }

        [TestMethod]
        public async Task RequestBroker_InvokeVoidTask()
        {
            IProxySubject controlInstance = new ProxySubject();

            var broker = new RequestBroker();
            var methodsRegistered = broker.AddService(controlInstance);
            var request = RequestTests.CreateControlVoidTaskRequest();

            var response = await broker.InvokeAsync(request);

            Assert.AreEqual(true, response.Success);
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(request.Id, response.RequestId);
            Assert.IsNull(response.ReturnValue);
        }

        [TestMethod]
        public async Task RequestBroker_InvokeVoid()
        {
            IProxySubject controlInstance = new ProxySubject();

            var broker = new RequestBroker();
            var methodsRegistered = broker.AddService(controlInstance);
            var request = RequestTests.CreateControlVoidRequest();

            var response = await broker.InvokeAsync(request);

            Assert.AreEqual(true, response.Success);
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(request.Id, response.RequestId);
            Assert.IsNull(response.ReturnValue);
        }
    }
}
