using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Networking.AsyncMessaging.Tests.RequestProxyTests;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Utils;
using LagoVista.Core.PlatformSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.ProxyTests
{
    [TestClass]
    public class AsyncProxyFactoryTests
    {
        private readonly ILogger _logger = new TestLogger();
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
            var proxyFactory = new AsyncProxyFactory(
                new FakeConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);

            var proxy = proxyFactory.Create<IProxySubject>(
                _orgId,
                _insId,
                TimeSpan.FromSeconds(10));
            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyFactory_Create_ReturnsCorrectInterface()
        {
            var proxyFactory = new AsyncProxyFactory(
                new FakeConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                _orgId,
                _insId,
                TimeSpan.FromSeconds(10));
            Assert.IsInstanceOfType(proxy, typeof(IProxySubject));
        }

        [TestMethod]
        public void ProxyFactory_Create_InterfaceContainsMethods()
        {
            var proxyFactory = new AsyncProxyFactory(
                new FakeConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                _orgId,
                _insId,
                TimeSpan.FromSeconds(10));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.Echo)));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.EchoAsync)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullOrganization()
        {
            var proxyFactory = new AsyncProxyFactory(
                new FakeConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                (string)null,
                _insId,
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullInstance()
        {
            var proxyFactory = new AsyncProxyFactory(
                new FakeConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                _orgId,
                (string)null,
                TimeSpan.FromSeconds(10));
        }
    }
}