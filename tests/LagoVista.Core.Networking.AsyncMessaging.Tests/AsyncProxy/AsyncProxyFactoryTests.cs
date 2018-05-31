using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncProxyTests
{
    [TestClass]
    public class AsyncProxyFactoryTests: TestBase
    {
        protected readonly IAsyncProxyFactory _proxyFactory = new AsyncProxyFactory();
        protected readonly Mock<IAsyncCoupler<IAsyncResponse>> _successCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        protected readonly Mock<IAsyncCoupler<IAsyncResponse>> _failCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        private readonly Mock<IAsyncRequestHandler> _sender = new Mock<IAsyncRequestHandler>();
        private readonly string _destination = "over the rainbow";

         [TestMethod]
        public void ProxyFactory_Create_ReturnsInstance()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object, 
                _sender.Object, 
                logger,
                _destination,
                //metrics, 
                TimeSpan.FromSeconds(10));
            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyFactory_Create_ReturnsCorrectInterface()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object, 
                _sender.Object, 
                logger,
                _destination,
                //metrics, 
                TimeSpan.FromSeconds(10));
            Assert.IsInstanceOfType(proxy, typeof(IProxySubject));
        }

        [TestMethod]
        public void ProxyFactory_Create_InterfaceContainsMethods()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object, 
                _sender.Object, 
                logger,
                _destination,
                //metrics, 
                TimeSpan.FromSeconds(10));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.Echo)));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.EchoAsync)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullAsyncProxy()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(null, 
                _sender.Object, 
                logger,
                _destination,
                //metrics, 
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullRequestSender()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object, 
                null, 
                logger,
                _destination,
                //metrics, 
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullLogger()
        {
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object, 
                _sender.Object, 
                null,
                _destination,
                //metrics, 
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullDestination()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object,
                _sender.Object,
                logger,
                null,
                //metrics, 
                TimeSpan.FromSeconds(10));
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void ProxyFactory_Create_NullUsageMetrics()
        //{
        //    var logger = new TestLogger();
        //    var proxy = _proxyFactory.Create<IProxySubject>(_successCoupler.Object, _sender.Object, logger, null, TimeSpan.FromSeconds(10));
        //}
    }
}