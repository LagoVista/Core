﻿using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Utils;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncProxyTests
{

    [TestClass]
    public class AsyncProxyTests : AsyncProxyFactoryTests
    {
        private readonly IAsyncCoupler<IAsyncResponse> _coupler = new AsyncCoupler<IAsyncResponse>(new TestLogger(), new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" });
        private readonly ILogger _logger = new TestLogger();
        private readonly IUsageMetrics _metrics = new TestUsageMetrics("rpc", "rcp", "rpc") { Version = "N/A" };
        private FakeSender _sender;
        private IProxySubject _proxySubject;

        [TestInitialize]
        public void Init()
        {
            _sender = new FakeSender(_coupler, ProxySubject.EchoValueConst);
            _proxySubject = _proxyFactory.Create<IProxySubject>(_coupler, _sender, _logger, _metrics, TimeSpan.FromSeconds(30));
            
            //controlEchoSuccessResponse = new AsyncResponse(controlEchoRequest, ProxySubject.EchoValueConst);
            //successCoupler.Setup(mock => mock.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
            //    Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(controlEchoSuccessResponse)));

            //controlEchoFailureResponse = new AsyncResponse(controlEchoRequest, responseFailException);
            //failCoupler.Setup(mock => mock.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
            //    Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(controlEchoFailureResponse)));

            //_successCoupler.Setup(proc => proc.CompleteAsync(It.IsAny<string>(), It.IsAny<IAsyncResponse>())).Returns(Task.FromResult(InvokeResult.Success));

            //_sender.Setup(proc => proc.HandleRequest(It.IsAny<IAsyncRequest>())).Returns(Task.FromResult<object>(null));
        }

        [TestMethod]
        public void AsyncProxy_Echo_ResultIsNotNull()
        {
            var echoResult = _proxySubject.Echo(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public void AsyncProxy_Echo_ResultIsCorrectValue()
        {
            var echoResult = _proxySubject.Echo(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public async Task AsyncProxy_EchoAsync_ResultIsNotNull()
        {
            var echoResult = await _proxySubject.EchoAsync(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public async Task AsyncProxy_EchoAsync_ResultIsCorrectValue()
        {
            var echoResult = await _proxySubject.EchoAsync(ProxySubject.EchoValueConst);
            Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
        }

        [TestMethod]
        public void AsyncProxy_PassStringParams_ResultIsCorrectValue1()
        {
            var array = new string[] { ProxySubject.EchoValueConst };
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(array);
            var sender = new FakeSender(_coupler, result);
            var proxySubject = _proxyFactory.Create<IProxySubject>(_coupler, sender, _logger, _metrics, TimeSpan.FromSeconds(30));
            var echoResult = proxySubject.PassStringParams(array);

            Assert.AreEqual(result, echoResult);
        }

        [TestMethod]
        public void AsyncProxy_PassStringParams_ResultIsCorrectValue2()
        {
            var array = new string[] { ProxySubject.EchoValueConst };
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(array);
            var sender = new FakeSender(_coupler, result);
            var proxySubject = _proxyFactory.Create<IProxySubject>(_coupler, sender, _logger, _metrics, TimeSpan.FromSeconds(30));
            var methodResult = proxySubject.PassStringParams(ProxySubject.EchoValueConst);

            Assert.AreEqual(result, methodResult);
            //var subject = new ProxySubject();
            //var json1 = subject.PassStringParams(ProxySubject.EchoValueConst);
            //var json2 = subject.PassStringParams(array);
        }
    }
}
