using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncProxyTests
{
    [TestClass]
    public class AsyncProxyFactoryTests: TestBase
    {
        protected readonly IAsyncProxyFactory proxyFactory = new AsyncProxyFactory();
        protected readonly Mock<IAsyncCoupler<IAsyncResponse>> successCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        protected readonly Mock<IAsyncCoupler<IAsyncResponse>> failCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        protected readonly Mock<IAsyncRequestHandler> sender = new Mock<IAsyncRequestHandler>();

         [TestMethod]
        public void ProxyFactory_Create_ReturnsInstance()
        {
            var proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, sender.Object);
            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        public void ProxyFactory_Create_ReturnsCorrectInterface()
        {
            var proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, sender.Object);
            Assert.IsInstanceOfType(proxy, typeof(IProxySubject));
        }

        [TestMethod]
        public void ProxyFactory_Create_InterfaceContainsMethods()
        {
            var proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, sender.Object);
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.Echo)));
            Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.EchoAsync)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullAsyncProxy()
        {
            var proxy = proxyFactory.Create<IProxySubject>(null, sender.Object);
            Assert.IsNotNull(proxy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProxyFactory_Create_NullRequestSender()
        {
            var proxy = proxyFactory.Create<IProxySubject>(successCoupler.Object, null);
            Assert.IsNotNull(proxy);
        }
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
