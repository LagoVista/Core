using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Client
{

    [TestClass]
    public class ProxyTests
    {
        private readonly IProxySubject _proxySubect;
        private readonly IProxyFactory _proxyFactory = ProxyFactoryTests.CreateControlProxyFactory();

        public ProxyTests()
        {
            _proxySubect = _proxyFactory.Create<IProxySubject>(Constants.ProxySettings);
        }

        [TestMethod]
        public void Proxy_VoidMethod()
        {
            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                logger.Object,
                null);
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
            var proxy = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);
            proxy.VoidMethod();
        }

        [TestMethod]
        public async Task Proxy_VoidTaskMethod()
        {
            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                logger.Object,
                null);
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
            var proxy = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);
            await proxy.VoidTaskMethod();
        }

        [TestMethod]
        public void Proxy_Echo_ResultIsNotNull()
        {
            var echoResult = _proxySubect.Echo(ProxySubject.EchoValueConst);
            Assert.IsNotNull(echoResult);
        }

        [TestMethod]
        public void Proxy_Echo_ResultIsCorrectValue()
        {
            var echoResult = _proxySubect.Echo(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public async Task Proxy_EchoAsync_ResultIsNotNull()
        {
            var echoResult = await _proxySubect.EchoAsync(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public async Task Proxy_EchoAsync_ResultIsCorrectValue()
        {
            var echoResult = await _proxySubect.EchoAsync(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Proxy_EchoAsync_MethodNotSupported()
        {
            var echoResult = _proxySubect.SkipMe();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Proxy_PassStringParams_PassingArrayOfArgs_ResultIsCorrectValue()
        {
            var array = new string[] { ProxySubject.EchoValueConst };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(array);

            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                logger.Object,
                json);
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
            var proxySubject = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);

            var echoResult = proxySubject.PassStringParams(array);
            Assert.AreEqual(json, echoResult);
        }

        [TestMethod]
        public void Proxy_IgnoreParametersAtInterface()
        {
            var array = new string[] { ProxySubject.EchoValueConst };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(array);

            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                logger.Object,
                json);
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
            var proxySubject = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);

            var methodResult = proxySubject.IgnoreParametersAtInterface("param1", "param2");
        }

        [TestMethod]
        public void Proxy_IgnoreParametersAtImplementation()
        {
            var array = new string[] { ProxySubject.EchoValueConst };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(array);

            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                logger.Object,
                json);
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
            var proxySubject = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);

            var methodResult = proxySubject.IgnoreParametersAtImplementation("param1", "param2");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Proxy_PassStringParams_PassingSingleArgIntoParamsMethod_ResultIsCorrectValue2()
        {
            var array = new string[] { ProxySubject.EchoValueConst };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(array);

            var logger = new Mock<ILogger>();
            var client = new SimulatedTransceiver(
                Constants.ConnectionSettings,
                Constants.AsyncCoupler,
                logger.Object,
                json);
            var proxyFactory = new ProxyFactory(Constants.ConnectionSettings, client, Constants.AsyncCoupler, logger.Object);
            var proxySubject = proxyFactory.Create<IProxySubject>(Constants.ProxySettings);

            var methodResult = proxySubject.PassStringParams(ProxySubject.EchoValueConst);
            Assert.AreEqual(json, methodResult);

            // shows that the result of json serialization is the same as passing an array or a single arg
            // just confirmation that the test code was written correctly
            //var subject = new ProxySubject();
            //var json1 = subject.PassStringParams(ProxySubject.EchoValueConst);
            //var json2 = subject.PassStringParams(array);
        }
    }
}
