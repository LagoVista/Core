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

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void ProxyFactory_Create_NullUsageMetrics()
        //{
        //    var logger = new TestLogger();
        //    var proxy = _proxyFactory.Create<IProxySubject>(_successCoupler.Object, _sender.Object, logger, null, TimeSpan.FromSeconds(10));
        //}
    }
}



//protected void WriteResult(ListResponse<DataStreamResult> response)
//{
//    var idx = 1;
//    foreach (var item in response.Model)
//    {
//        Console.WriteLine($"Record {idx++} - {item.Timestamp}");

//        foreach (var fld in item)
//        {
//            Console.WriteLine($"\t{fld.Key} - {fld.Value}");
//        }
//        Console.WriteLine("----");
//        Console.WriteLine();
//    }
//}

//protected void AssertInvalidError(InvokeResult result, params string[] errs)
//{
//    Console.WriteLine("Errors (at least some are expected)");

//    foreach (var err in result.Errors)
//    {
//        Console.WriteLine(err.Message);
//    }

//    foreach (var err in errs)
//    {
//        Assert.IsTrue(result.Errors.Where(msg => msg.Message == err).Any(), $"Could not find error [{err}]");
//    }

//    Assert.AreEqual(errs.Length, result.Errors.Count, "Validation error mismatch between");

//    Assert.IsFalse(result.Successful, "Validated as successful but should have failed.");
//}

//protected void AssertSuccessful(InvokeResult result)
//{
//    if (result.Errors.Any())
//    {
//        Console.WriteLine("unexpected errors");
//    }

//    foreach (var err in result.Errors)
//    {
//        Console.WriteLine("\t" + err.Message);
//    }

//    Assert.IsTrue(result.Successful);
//}
