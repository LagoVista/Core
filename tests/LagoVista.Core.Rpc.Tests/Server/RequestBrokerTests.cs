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
        private readonly IProxySubject _controlInstance = new ProxySubject();

        [TestMethod]
        public void RequestBroker_RegisterSubject_Success()
        {
            var broker = new RequestBroker();
            var methodsRegistered = broker.AddService(_controlInstance);

            var instanceMethodCount = typeof(IProxySubject).GetMethods().Count();
            Assert.AreEqual(5, instanceMethodCount);
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
            var broker = new RequestBroker();
            var methodsRegistered = broker.AddService(_controlInstance);
            var request = RequestTests.CreateControlEchoRequest();

            var response = await broker.InvokeAsync(request);

            Assert.AreEqual(true, response.Success);
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(request.Id, response.RequestId);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.ReturnValue);
        }
    }
}
