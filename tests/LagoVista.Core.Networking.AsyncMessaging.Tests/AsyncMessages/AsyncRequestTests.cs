using LagoVista.Core.Networking.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Networking.Rpc.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncRequestTests
    {
        private static readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private static readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private static readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        private static readonly string _echoMethodParamName = "value";
        private static readonly string _responseValue = "jello babies";
        private static readonly string _rootExceptionValue = "boo";
        private static readonly string _orgId = "orgid";
        private static readonly string _insId = "insid";
        private static readonly string _replyPath = "replyPath";

        private static IAsyncRequest CreateControlEchoRequest()
        {
            return new AsyncRequest(_echoMethodInfo, _echoArgs, _orgId, _insId, _replyPath);
        }

        private static IAsyncResponse CreateControlEchoSuccessResponse()
        {
            var request = CreateControlEchoRequest();
            return new AsyncResponse(request, _responseValue);
        }

        private static IAsyncResponse CreateControlEchoFailureResponse()
        {
            var request = CreateControlEchoRequest();
            var ex = new Exception(_rootExceptionValue, new Exception("hoo"));
            return new AsyncResponse(request, ex);
        }
        [TestMethod]
        public void AsyncRequest_Constructor_MethodInfo_Args()
        {
            var request = new AsyncRequest(_echoMethodInfo, _echoArgs, _orgId, _insId, _replyPath);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncRequest_Constructor_MethodInfo_NullArgument()
        {
            MethodInfo echoMethodInfo = null;
            var request = new AsyncRequest(echoMethodInfo, _echoArgs, _orgId, _insId, _replyPath);
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
            Assert.AreEqual(controlRequest.DestinationPath, request.DestinationPath);
            Assert.AreEqual(controlRequest.TimeStamp, request.TimeStamp);
            Assert.AreEqual(controlRequest.Json, request.Json);
            Assert.IsTrue(controlRequest.Payload.SequenceEqual(request.Payload));
        }

        [TestMethod]
        public void AsyncRequest_Constructor_MethodInfo_SimpleArg()
        {
            var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new AsyncRequest(echoMethodInfo, _echoArgs, _orgId, _insId, _replyPath);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AsyncRequest_Constructor_MethodInfo_ParamsAr_ThrowsNotSupportedException()
        {
            var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.PassStringParams));
            var request = new AsyncRequest(echoMethodInfo, _echoArgs, _orgId, _insId, _replyPath);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToCountMismatch()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new AsyncRequest(methodInfo, new object[] { ProxySubject.EchoValueConst, new object(), null }, _orgId, _insId, _replyPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToTypeMismatch()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new AsyncRequest(methodInfo, new object[] { 3 }, _orgId, _insId, _replyPath);
        }
    }
}
