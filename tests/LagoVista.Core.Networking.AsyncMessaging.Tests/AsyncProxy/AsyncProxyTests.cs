using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncProxy
{
    [TestClass]
    public sealed class AsyncProxyTests
    {
        private readonly IAsyncProxyFactory _proxyFactory = new AsyncProxyFactory();
        private readonly Mock<IAsyncCoupler<IAsyncResponse>> _coupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        private readonly Mock<IAsyncRequestHandler> _sender = new Mock<IAsyncRequestHandler>();

        [TestInitialize]
        public void Init()
        {
            _coupler.Setup(proc => proc.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
                Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(new AsyncResponse(EchoTest.EchoValueConst))));
            _coupler.Setup(proc => proc.CompleteAsync(It.IsAny<string>(), It.IsAny<IAsyncResponse>())).Returns(Task.FromResult(InvokeResult.Success));

            _sender.Setup(proc => proc.HandleRequest(It.IsAny<IAsyncRequest>())).Returns(Task.FromResult<object>(null));
        }

        [TestMethod]
        public async Task TestAsyncProxy()
        {
            IEchoTest proxy = _proxyFactory.Create<IEchoTest>(_coupler.Object, _sender.Object);

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOfType(proxy, typeof(IEchoTest));

            Assert.IsNotNull(proxy.GetType().GetMethod("Echo"));
            Assert.IsNotNull(proxy.GetType().GetMethod("EchoAsync"));

            var echoResult = await proxy.EchoAsync(EchoTest.EchoValueConst);
            Assert.IsNotNull(echoResult);
            Assert.AreEqual(EchoTest.EchoValueConst, echoResult);

            echoResult = proxy.Echo(EchoTest.EchoValueConst);
            Assert.IsNotNull(echoResult);
            Assert.AreEqual(EchoTest.EchoValueConst, echoResult);
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
