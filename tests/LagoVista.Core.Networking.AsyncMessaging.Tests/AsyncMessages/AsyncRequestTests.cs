using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncRequestTests : TestBase
    {
        private readonly MethodInfo echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private readonly object[] echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private readonly string echoMethodParam = "value";

        private IAsyncRequest CreateControlRequest()
        {
            return new AsyncRequest(echoMethodInfo, echoArgs);
        }

        [TestMethod]
        public void AsyncRequest_Constructor_MethodInfo_Args()
        {
            var request = new AsyncRequest(echoMethodInfo, echoArgs);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(echoMethodParam));
            Assert.AreEqual(1, request.ArgumentCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncRequest_Constructor_MethodInfo_NullArgument()
        {
            MethodInfo echoMethodInfo = null;
            var request = new AsyncRequest(echoMethodInfo, echoArgs);
        }

        [TestMethod]
        public void AsyncRequest_Constructor_MarshalledData()
        {
            var controlRequest = CreateControlRequest();

            var request = new AsyncRequest(controlRequest.MarshalledData);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(echoMethodParam));
            Assert.AreEqual(1, request.ArgumentCount);

            Assert.AreEqual(controlRequest.Id, request.Id);
            Assert.AreEqual(controlRequest.CorrelationId, request.CorrelationId);
            Assert.AreEqual(controlRequest.Path, request.Path);
            Assert.AreEqual(controlRequest.TimeStamp, request.TimeStamp);
            Assert.AreEqual(controlRequest.Json, request.Json);
            Assert.IsTrue(controlRequest.MarshalledData.SequenceEqual(request.MarshalledData));
        }
    }
}
