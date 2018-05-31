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
        [TestMethod]
        public void AsyncRequest_Constructor_MethodInfo_Args()
        {
            var request = new AsyncRequest(_echoMethodInfo, _echoArgs);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncRequest_Constructor_MethodInfo_NullArgument()
        {
            MethodInfo echoMethodInfo = null;
            var request = new AsyncRequest(echoMethodInfo, _echoArgs);
        }

        [TestMethod]
        public void AsyncRequest_Constructor_MarshalledData()
        {
            var controlRequest = CreateControlEchoRequest();

            var request = new AsyncRequest(controlRequest.Payload);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);

            Assert.AreEqual(controlRequest.Id, request.Id);
            Assert.AreEqual(controlRequest.CorrelationId, request.CorrelationId);
            Assert.AreEqual(controlRequest.Path, request.Path);
            Assert.AreEqual(controlRequest.TimeStamp, request.TimeStamp);
            Assert.AreEqual(controlRequest.Json, request.Json);
            Assert.IsTrue(controlRequest.Payload.SequenceEqual(request.Payload));
        }

        [TestMethod]
        public void AsyncRequest_Constructor_MethodInfo_SimpleArg()
        {
            var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new AsyncRequest(echoMethodInfo, _echoArgs);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AsyncRequest_Constructor_MethodInfo_ParamsAr_ThrowsNotSupportedException()
        {
            var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.PassStringParams));
            var request = new AsyncRequest(echoMethodInfo, _echoArgs);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToCountMismatch()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new AsyncRequest(methodInfo, new object[] { ProxySubject.EchoValueConst, new object(), null });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToTypeMismatch()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new AsyncRequest(methodInfo, new object[] { 3 });
        }
    }
}
