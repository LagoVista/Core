using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncResponseTests 
    {
        private static readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private static readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private static readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        private static readonly string _responseValue = "jello babies";
        private static readonly string _rootExceptionValue = "boo";

        private static IAsyncRequest CreateControlEchoRequest()
        {
            return new AsyncRequest(_echoMethodInfo, _echoArgs);
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

        private void AssertSuccessResponse(IAsyncResponse response)
        {
            Assert.IsTrue(response.Success);
            Assert.IsNull(response.Exception);
        }

        [TestMethod]
        public void AsyncResponse_Constructor_StandardSuccessResponse()
        {
            var controlRequest = CreateControlEchoRequest();
            var response = new AsyncResponse(controlRequest, _responseValue);

            AssertSuccessResponse(response);
            Assert.AreEqual(controlRequest.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlRequest.Path, response.Path);
            Assert.AreNotEqual(controlRequest.TimeStamp, response.TimeStamp);
            Assert.AreNotEqual(controlRequest.Id, response.Id);
            Assert.AreEqual(controlRequest.Id, response.RequestId);
            Assert.AreEqual(_responseValue, response.ReturnValue);
            Assert.AreEqual(_responseValue, response.GetTypedReturnValue<string>());

        }

        [TestMethod]
        public void AsyncResponse_Constructor_MarshalledData_SuccessResponse()
        {
            var controlResponse = CreateControlEchoSuccessResponse();
            var response = new AsyncResponse(controlResponse.Payload);

            AssertSuccessResponse(response);
            Assert.AreEqual(controlResponse.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlResponse.Path, response.Path);
            Assert.AreEqual(controlResponse.TimeStamp, response.TimeStamp);
            Assert.AreEqual(controlResponse.Id, response.Id);
            Assert.AreEqual(controlResponse.ReturnValue, response.ReturnValue);
            Assert.AreEqual(controlResponse.GetTypedReturnValue<string>(), response.GetTypedReturnValue<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncResponse_Constructor_IAsyncRequest_NullArgument()
        {
            IAsyncRequest request = null;
            var response = new AsyncResponse(request, _responseValue);
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void AsyncResponse_Constructor_Object_NullArgument()
        //{
        //    var request = CreateControlRequest();
        //    object responseValue = null;
        //    var response = new AsyncResponse(request, responseValue);
        //}

        private void AssertFailureResponse(IAsyncResponse response)
        {
            Assert.IsFalse(response.Success);
            Assert.IsNotNull(response.Exception);
            Assert.IsNull(response.ReturnValue);
        }

        [TestMethod]
        public void AsyncResponse_Constructor_StandardFailureResponse()
        {
            var controlRequest = CreateControlEchoRequest();
            var ex = new Exception(_rootExceptionValue, new Exception("hoo"));

            var response = new AsyncResponse(controlRequest, ex);
            AssertFailureResponse(response);

            Assert.AreEqual(controlRequest.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlRequest.Path, response.Path);
            Assert.AreNotEqual(controlRequest.TimeStamp, response.TimeStamp);
            Assert.AreNotEqual(controlRequest.Id, response.Id);
            Assert.AreEqual(controlRequest.Id, response.RequestId);
        }

        [TestMethod]
        public void AsyncResponse_Constructor_MarshalledData_FailureResponse()
        {
            var controlResponse = CreateControlEchoFailureResponse();
            var response = new AsyncResponse(controlResponse.Payload);

            AssertFailureResponse(response);

            Assert.AreEqual(_rootExceptionValue, response.Exception.Message);
            Assert.AreEqual(controlResponse.Exception.Message, response.Exception.Message);
            Assert.AreEqual(controlResponse.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlResponse.Path, response.Path);
            Assert.AreEqual(controlResponse.TimeStamp, response.TimeStamp);
            Assert.AreEqual(controlResponse.Id, response.Id);
            Assert.AreEqual(controlResponse.RequestId, response.RequestId);
        }
    }
}
