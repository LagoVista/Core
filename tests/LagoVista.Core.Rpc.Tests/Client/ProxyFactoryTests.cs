// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2071d5ddef8380f688d8ef1c41e2ddf67a917400ff0c1d8e8edb6e1e5e249f51
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        public void ProxyFactory_Create_NullProxySettings()
        {

            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var proxyFactory = CreateControlProxyFactory();
                var proxy = proxyFactory.Create<IProxySubject>(null);
            });
        }

        [TestMethod]
        public void ProxyFactory_Constructor_NullConnectionSettings()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var client = new Mock<ITransceiver>();
                var coupler = new Mock<IAsyncCoupler<IMessage>>();
                var logger = new Mock<ILogger>();
                var proxyFactory = new ProxyFactory(null, client.Object, Constants.AsyncCoupler, logger.Object);
            });
        }

        [TestMethod]
        public void ProxyFactory_Constructor_NullClient()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var coupler = new Mock<IAsyncCoupler<IMessage>>();
                var logger = new Mock<ILogger>();
                var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, null, Constants.AsyncCoupler, logger.Object);
            });
        }

        [TestMethod]
        public void ProxyFactory_Constructor_NullCoupler()
        {

            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var client = new Mock<ITransceiver>();
                var logger = new Mock<ILogger>();
                var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client.Object, null, logger.Object);
            });
        }

        [TestMethod]
        public void ProxyFactory_Constructor_NullLogger()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var client = new Mock<ITransceiver>();
                var coupler = new Mock<IAsyncCoupler<IMessage>>();
                var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client.Object, Constants.AsyncCoupler, null);
            });
        }
    }
}