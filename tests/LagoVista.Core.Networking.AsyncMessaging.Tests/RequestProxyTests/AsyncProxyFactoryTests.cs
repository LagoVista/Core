using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.ProxyTests
{
    [TestClass]
    public class AsyncProxyFactoryTests
    {
        protected readonly IAsyncProxyFactory _proxyFactory = new AsyncProxyFactory(new FakeConnectionSettings());
        protected readonly Mock<IAsyncCoupler<IAsyncResponse>> _successCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        protected readonly Mock<IAsyncCoupler<IAsyncResponse>> _failCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        private readonly Mock<IAsyncRequestHandler> _sender = new Mock<IAsyncRequestHandler>();
        private static readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private static readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private static readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        private static readonly string _responseValue = "jello babies";
        private static readonly string _rootExceptionValue = "boo";
        private static readonly string _orgId = "orgid";
        private static readonly string _insId = "insid";
        private static readonly string _replyPath = "replyPath";

        public class FakeConnectionSettings : IServiceBusAsyncResponseListenerConnectionSettings
        {
            public IConnectionSettings ServiceBusAsyncResponseListener { get; set; } = new ConnectionSettings()
            {
                ResourceName = _replyPath
            };
        }

        private static IAsyncRequest CreateControlEchoRequest()
        {
            return new AsyncRequest(_echoMethodInfo, _echoArgs, _orgId, _insId, _replyPath);
        }

        private static IAsyncResponse CreateControlEchoSuccessResponse()
        {
            var request = CreateControlEchoRequest();
            return new AsyncResponse(request, _responseValue);
        }

        private static IAsyncResponse CreateControlEchoFailureResponse()
        {
            var request = CreateControlEchoRequest();
            var ex = new Exception(_rootExceptionValue, new Exception("hoo"));
            return new AsyncResponse(request, ex);
        }
        [TestMethod]
        public void ProxyFactory_Create_ReturnsInstance()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object,
                _sender.Object,
                logger,
                _orgId,
                _insId,
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
                _orgId,
                _insId,
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
                _orgId,
                _insId,
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
                _orgId,
                _insId,
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
                _orgId,
                _insId,
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
                _orgId,
                _insId,
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullOrganization()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object,
                _sender.Object,
                logger,
                (string)null,
                _insId,
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullInstance()
        {
            var logger = new TestLogger();
            var metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
            var proxy = _proxyFactory.Create<IProxySubject>(
                _successCoupler.Object,
                _sender.Object,
                logger,
                _orgId,
                (string)null,
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