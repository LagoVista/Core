using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LagoVista.Core.Rpc.Tests.RequestBrokerTests
{
    [TestClass]
    public class RequestBrokerTests
    {
        private readonly IProxySubject _controlInstance = new ProxySubject();
        //private readonly MethodInfo _methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        //private readonly MethodInfo _asyncMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.EchoAsync));
        //private InstanceMethodPair _pair = null;
        //private InstanceMethodPair _asyncPair = null;

        //[TestInitialize]
        //public void Init()
        //{
        //    Assert.IsNotNull(_methodInfo);
        //    Assert.IsNotNull(_asyncMethodInfo);

        //    _pair = new InstanceMethodPair(_instance, _methodInfo);
        //    _asyncPair = new InstanceMethodPair(_instance, _asyncMethodInfo);
        //}

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

    }
}
