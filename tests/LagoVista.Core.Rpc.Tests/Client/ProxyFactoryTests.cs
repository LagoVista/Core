using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace LagoVista.Core.Rpc.Tests.Client
{
    [TestClass]
    public class ProxyFactoryTests
    {
        public static IProxyFactory CreateControlProxyFactory()
        {
            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.AsyncCoupler,
                logger.Object,
                ProxySubject.EchoValueConst);
            return new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
        }

        [TestMethod]
        public void ProxyFactory_Create_ReturnsInstance()
        {
            var proxyFactory = CreateControlProxyFactory();
            var proxy = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);
            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyFactory_Create_ReturnsCorrectInterface()
        {
            var proxyFactory = CreateControlProxyFactory();
            var proxy = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);
            Assert.IsInstanceOfType(proxy, typeof(IProxySubject));
        }

        [TestMethod]
        public void ProxyFactory_Create_InterfaceContainsMethods()
        {
            var proxyFactory = CreateControlProxyFactory();
            var proxy = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.Echo)));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.EchoAsync)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullProxySettings()
        {
            var proxyFactory = CreateControlProxyFactory();
            var proxy = proxyFactory.Create<IProxySubject>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Constructor_NullConnectionSettings()
        {
            var client = new Mock<ITransceiver>();
            var coupler = new Mock<IAsyncCoupler<IMessage>>();
            var logger = new Mock<ILogger>();
            var proxyFactory = new ProxyFactory(null, client.Object, Constants.AsyncCoupler, logger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Constructor_NullClient()
        {
            var coupler = new Mock<IAsyncCoupler<IMessage>>();
            var logger = new Mock<ILogger>();
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, null, Constants.AsyncCoupler, logger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Constructor_NullCoupler()
        {
            var client = new Mock<ITransceiver>();
            var logger = new Mock<ILogger>();
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client.Object, null, logger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Constructor_NullLogger()
        {
            var client = new Mock<ITransceiver>();
            var coupler = new Mock<IAsyncCoupler<IMessage>>();
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client.Object, Constants.AsyncCoupler, null);
        }
    }
}