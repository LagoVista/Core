using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using LagoVista.Core.Validation;
using LagoVista.Core.Networking.AsyncMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncProxyTests
{
    [TestClass]
    public sealed class AsyncProxyTests
    {
    //    private readonly IAsyncProxyFactory _proxyFactory = new AsyncProxyFactory();
    //    private readonly Mock<IAsyncCoupler<IAsyncResponse>> _coupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
    //    private readonly Mock<IAsyncRequestHandler> _sender = new Mock<IAsyncRequestHandler>();

    //    [TestInitialize]
    //    public void Init()
    //    {
    //        _coupler.Setup(proc => proc.WaitOnAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).
    //            Returns(Task.FromResult(InvokeResult<IAsyncResponse>.Create(new AsyncResponse(ProxySubject.EchoValueConst))));
    //        _coupler.Setup(proc => proc.CompleteAsync(It.IsAny<string>(), It.IsAny<IAsyncResponse>())).Returns(Task.FromResult(InvokeResult.Success));

    //        _sender.Setup(proc => proc.HandleRequest(It.IsAny<IAsyncRequest>())).Returns(Task.FromResult<object>(null));
    //    }


    //    [TestMethod]
    //    public void ProxyFactory_CreateShouldReturnNotNullInstance()
    //    {
    //        var proxy = _proxyFactory.Create<IProxySubject>(_coupler.Object, _sender.Object);
    //        Assert.IsNotNull(proxy);
    //    }

    //    [TestMethod]
    //    public void ProxyFactory_CreateShouldReturnCorrectInterface()
    //    {
    //        var proxy = _proxyFactory.Create<IProxySubject>(_coupler.Object, _sender.Object);
    //        Assert.IsInstanceOfType(proxy, typeof(IProxySubject));
    //    }

    //    [TestMethod]
    //    public void ProxyFactory_InterfaceShouldContainsMethods()
    //    {
    //        var proxy = _proxyFactory.Create<IProxySubject>(_coupler.Object, _sender.Object);
    //        Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.Echo)));
    //        Assert.IsNotNull(proxy.GetType().GetMethod(nameof(IProxySubject.EchoAsync)));
    //    }

    //    [TestMethod]
    //    public async Task Proxy_InterfaceShouldContainsMethods()
    //    {
    //        IProxySubject proxy = _proxyFactory.Create<IProxySubject>(_coupler.Object, _sender.Object);

    //        var echoResult = await proxy.EchoAsync(ProxySubject.EchoValueConst);
    //        Assert.IsNotNull(echoResult);
    //        Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);

    //        var targetMethod = typeof(IProxySubject).GetMethod(nameof(IProxySubject.Echo));
    //        var asyncRequest = ((AsyncProxy)proxy).CreateAsyncRequest(targetMethod);

    //        //var parameters = targetMethod.GetParameters();
    //        //((AsyncProxy)proxy).PopulateAsyncRequestParameters(asyncRequest, parameters, args);



    //        _sender.Verify(sndr=> sndr.HandleRequest(It.Is<IAsyncRequest>(arc => arc.ArgumentCount == 1 && arc.Path == "")));
    //        //_plannerQueue.Verify(pq => pq.EnqueueAsync(It.Is<PipelineExecutionMessage>(pem => pem.MessageId == "msgid" && pem.Envelope.DeviceId == "devid" && pem.TextPayload == "{\"hi\":\"there\"}")), Times.Once);
    //    }

    //    [TestMethod]
    //    public async Task TestAsyncProxy()
    //    {
    //        IProxySubject proxy = _proxyFactory.Create<IProxySubject>(_coupler.Object, _sender.Object);

    //        echoResult = proxy.Echo(ProxySubject.EchoValueConst);
    //        Assert.IsNotNull(echoResult);
    //        Assert.AreEqual(ProxySubject.EchoValueConst, echoResult);
    //    }
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
