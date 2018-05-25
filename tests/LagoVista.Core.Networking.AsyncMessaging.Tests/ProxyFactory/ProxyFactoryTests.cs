using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.ProxyFactory
{
    [TestClass]
    public sealed class ProxyFactoryTests
    {
        private readonly IAsyncProxyFactory _proxyFactory = new AsyncProxyFactory();
        private readonly Mock<IAsyncCoupler<IAsyncResponse>> _coupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        private readonly Mock<IAsyncRequestHandler> _sender = new Mock<IAsyncRequestHandler>();
        private readonly string _echoValue = "ping";

        public interface IProxyTest
        {
            Task<string> EchoAsync(string value);
            string Echo(string value);
        }

        [TestInitialize]
        public void Init()
        {
            _coupler.Setup(proc => proc.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
                Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(new AsyncResponse((object)_echoValue))));
            _coupler.Setup(proc => proc.CompleteAsync(It.IsAny<string>(), It.IsAny<IAsyncResponse>())).Returns(Task.FromResult(InvokeResult.Success));

            _sender.Setup(proc => proc.HandleRequest(It.IsAny<IAsyncRequest>())).Returns(Task.FromResult<object>(null));
        }

        [TestMethod]
        public async Task TestProxy()
        {
            var proxy = _proxyFactory.Create<IProxyTest>(_coupler.Object, _sender.Object);
            Assert.IsNotNull(proxy);
            Assert.IsNotNull(proxy.GetType().GetMethod("Echo"));
            Assert.IsNotNull(proxy.GetType().GetMethod("EchoAsync"));

            var echoResult = await proxy.EchoAsync(_echoValue);
            Assert.IsNotNull(echoResult);
            Assert.AreEqual(_echoValue, echoResult);

            echoResult = proxy.Echo(_echoValue);
            Assert.IsNotNull(echoResult);
            Assert.AreEqual(_echoValue, echoResult);
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
