using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncRequestBrokerTests
{
    [TestClass]
    public class AsyncRequestBrokerTests
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
        public void AsyncRequestBroker_RegisterSubject_Success()
        {
            var broker = new AsyncRequestBroker();
            var methodsRegistered = broker.RegisterSubject(_controlInstance);

            var instanceMethodCount = typeof(IProxySubject).GetMethods().Count();
            Assert.AreEqual(5, instanceMethodCount);
            Assert.AreEqual(instanceMethodCount - 1, methodsRegistered);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AsyncRequestBroker_RegisterSubject_FailureDueToNonInterfaceInstance()
        {
            var broker = new AsyncRequestBroker();
            broker.RegisterSubject(new ProxySubject());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncRequestBroker_RegisterSubject_FailureDueToNullInstance()
        {
            var broker = new AsyncRequestBroker();
            broker.RegisterSubject((IProxySubject)null);
        }

    }
}
