using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;

namespace LagoVista.Core.Rpc.Tests.Client
{
    [TestClass]
    public class ProxyFactoryTests
    {
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        private readonly Mock<IAsyncCoupler<IMessage>> _successCoupler = new Mock<IAsyncCoupler<IMessage>>();
        //private readonly Mock<IAsyncCoupler<IMessage>> _failCoupler = new Mock<IAsyncCoupler<IMessage>>();
        
        private readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        private readonly string _responseValue = "jello babies";
        private readonly string _rootExceptionValue = "boo";
        private readonly string Constants.OrganizationId = "orgid";
        private readonly string Constants.InstanceId = "insid";
        private readonly string Constants.MessageReplyPath = "replyPath";

        private readonly ProxySettings _proxySettings = new ProxySettings { InstanceId = ""};

        //private IRequest CreateControlEchoRequest()
        //{
        //    return new Request(_echoMethodInfo, _echoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        //}

        //private IResponse CreateControlEchoSuccessResponse()
        //{
        //    var request = CreateControlEchoRequest();
        //    return new Response(request, _responseValue);
        //}

        //private IResponse CreateControlEchoFailureResponse()
        //{
        //    var request = CreateControlEchoRequest();
        //    var ex = new Exception(_rootExceptionValue, new Exception("hoo"));
        //    return new Response(request, ex);
        //}
        private IProxyFactory CreateProxyFactory()
        {
            var connectionSettings = new Mock<ITransceiverConnectionSettings>();
            var client = new Mock<ITransceiver>();
            var coupler = new Mock<IAsyncCoupler<IMessage>>();
            var logger = new Mock<ILogger>();
            return new ProxyFactory(connectionSettings.Object, client.Object, coupler.Object, logger.Object);
        }


        [TestMethod]
        public void ProxyFactory_Create_ReturnsInstance()
        {
            var proxyFactory = CreateProxyFactory();

            var proxy = proxyFactory.Create<IProxySubject>(
                Constants.OrganizationId,
                Constants.InstanceId,
                TimeSpan.FromSeconds(10));
            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyFactory_Create_ReturnsCorrectInterface()
        {
            var proxyFactory = new ProxyFactory(
                new SimulatedConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                Constants.OrganizationId,
                Constants.InstanceId,
                TimeSpan.FromSeconds(10));
            Assert.IsInstanceOfType(proxy, typeof(IProxySubject));
        }

        [TestMethod]
        public void ProxyFactory_Create_InterfaceContainsMethods()
        {
            var proxyFactory = new ProxyFactory(
                new SimulatedConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                Constants.OrganizationId,
                Constants.InstanceId,
                TimeSpan.FromSeconds(10));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.Echo)));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.EchoAsync)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullOrganization()
        {
            var proxyFactory = new ProxyFactory(
                new SimulatedConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                (string)null,
                Constants.InstanceId,
                TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullInstance()
        {
            var proxyFactory = new ProxyFactory(
                new SimulatedConnectionSettings(),
                _successCoupler.Object,
                _sender.Object,
                _logger);
            var proxy = proxyFactory.Create<IProxySubject>(
                Constants.OrganizationId,
                (string)null,
                TimeSpan.FromSeconds(10));
        }
    }
}