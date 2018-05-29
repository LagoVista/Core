using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncResponseTests : TestBase
    {
        private readonly MethodInfo echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private readonly object[] echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private readonly string echoMethodParam = "value";
        private readonly string responseValue = "jello babies";
        private readonly string rootExceptionValue = "boo";

        private IAsyncRequest CreateControlRequest()
        {
            return new AsyncRequest(echoMethodInfo, echoArgs);
        }

        private IAsyncResponse CreateControlSuccessResponse()
        {
            var request = CreateControlRequest();
            return new AsyncResponse(request, responseValue);
        }

        private IAsyncResponse CreateControlFailResponse()
        {
            var request = CreateControlRequest();
            var ex = new Exception(rootExceptionValue, new Exception("hoo"));
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
            var controlRequest = CreateControlRequest();
            var response = new AsyncResponse(controlRequest, responseValue);

            AssertSuccessResponse(response);
            Assert.AreEqual(controlRequest.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlRequest.Path, response.Path);
            Assert.AreNotEqual(controlRequest.TimeStamp, response.TimeStamp);
            Assert.AreNotEqual(controlRequest.Id, response.Id);
            Assert.AreEqual(controlRequest.Id, response.RequestId);
            Assert.AreEqual(responseValue, response.ReturnValue);
            Assert.AreEqual(responseValue, response.GetTypedReturnValue<string>());

        }

        [TestMethod]
        public void AsyncResponse_Constructor_MarshalledData_SuccessResponse()
        {
            var controlResponse = CreateControlSuccessResponse();
            var response = new AsyncResponse(controlResponse.MarshalledData);

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
            var response = new AsyncResponse(request, responseValue);
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
            var controlRequest = CreateControlRequest();
            var ex = new Exception(rootExceptionValue, new Exception("hoo"));

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
            var controlResponse = CreateControlFailResponse();
            var response = new AsyncResponse(controlResponse.MarshalledData);

            AssertFailureResponse(response);

            Assert.AreEqual(rootExceptionValue, response.Exception.Message);
            Assert.AreEqual(controlResponse.Exception.Message, response.Exception.Message);
            Assert.AreEqual(controlResponse.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlResponse.Path, response.Path);
            Assert.AreEqual(controlResponse.TimeStamp, response.TimeStamp);
            Assert.AreEqual(controlResponse.Id, response.Id);
            Assert.AreEqual(controlResponse.RequestId, response.RequestId);
        }
    }
}
