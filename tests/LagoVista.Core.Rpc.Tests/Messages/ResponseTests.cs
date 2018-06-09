using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace LagoVista.Core.Rpc.Tests.Messages
{
    [TestClass]
    public class ResponseTests 
    {
        private readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        private readonly string _echoMethodParamName = "value";
        private readonly string _responseValue = "jello babies";
        private readonly string _rootExceptionValue = "boo";

        private readonly string _destination = "Test.Destination.Path";
        private readonly string _messageId = Guid.Parse("{C4CE5957-F9D7-4727-A20D-4C51AB5C6745}").ToString();
        private readonly string _messageCorrelationId = Guid.Parse("{1C2FC03B-3D21-42A3-97F3-1756177DE2CB}").ToString();
        private readonly string _organizationId = Guid.Parse("{8AF59E47-E473-41D1-AA86-8B557813EEFB}").ToString();
        private readonly string _instanceId = Guid.Parse("{EC0E2AE5-7B17-4C0D-9355-1903E3284FBE}").ToString();
        private readonly string _replyPath = "Test.ReplyTo.Path";
        private readonly DateTime _messageTimeStamp = new DateTime(2018, 1, 1, 13, 30, 30);

        private IRequest CreateControlEchoRequest()
        {
            return new Request(_echoMethodInfo, _echoArgs, _organizationId, _instanceId, _replyPath);
        }

        private IResponse CreateControlEchoSuccessResponse()
        {
            var request = CreateControlEchoRequest();
            return new Response(request, _responseValue);
        }

        private IResponse CreateControlEchoFailureResponse()
        {
            var request = CreateControlEchoRequest();
            var ex = new Exception(_rootExceptionValue, new Exception("hoo"));
            return new Response(request, ex);
        }

        private void AssertSuccessResponse(IResponse response)
        {
            Assert.IsTrue(response.Success);
            Assert.IsNull(response.Exception);
        }

        [TestMethod]
        public void Response_Constructor_StandardSuccessResponse()
        {
            var controlRequest = CreateControlEchoRequest();
            var response = new Response(controlRequest, _responseValue);

            AssertSuccessResponse(response);
            Assert.AreEqual(controlRequest.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlRequest.DestinationPath, response.DestinationPath);
            Assert.AreNotEqual(controlRequest.TimeStamp, response.TimeStamp);
            Assert.AreNotEqual(controlRequest.Id, response.Id);
            Assert.AreEqual(controlRequest.Id, response.RequestId);
            Assert.AreEqual(_responseValue, response.ReturnValue);
            Assert.AreEqual(_responseValue, response.GetTypedReturnValue<string>());

        }

        [TestMethod]
        public void Response_Constructor_MarshalledData_SuccessResponse()
        {
            var controlResponse = CreateControlEchoSuccessResponse();
            var response = new Response(controlResponse.Payload);

            AssertSuccessResponse(response);
            Assert.AreEqual(controlResponse.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlResponse.DestinationPath, response.DestinationPath);
            Assert.AreEqual(controlResponse.TimeStamp, response.TimeStamp);
            Assert.AreEqual(controlResponse.Id, response.Id);
            Assert.AreEqual(controlResponse.ReturnValue, response.ReturnValue);
            Assert.AreEqual(controlResponse.GetTypedReturnValue<string>(), response.GetTypedReturnValue<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Response_Constructor_IRequest_NullArgument()
        {
            IRequest request = null;
            var response = new Response(request, _responseValue);
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void Response_Constructor_Object_NullArgument()
        //{
        //    var request = CreateControlRequest();
        //    object responseValue = null;
        //    var response = new Response(request, responseValue);
        //}

        private void AssertFailureResponse(IResponse response)
        {
            Assert.IsFalse(response.Success);
            Assert.IsNotNull(response.Exception);
            Assert.IsNull(response.ReturnValue);
        }

        [TestMethod]
        public void Response_Constructor_StandardFailureResponse()
        {
            var controlRequest = CreateControlEchoRequest();
            var ex = new Exception(_rootExceptionValue, new Exception("hoo"));

            var response = new Response(controlRequest, ex);
            AssertFailureResponse(response);

            Assert.AreEqual(controlRequest.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlRequest.DestinationPath, response.DestinationPath);
            Assert.AreNotEqual(controlRequest.TimeStamp, response.TimeStamp);
            Assert.AreNotEqual(controlRequest.Id, response.Id);
            Assert.AreEqual(controlRequest.Id, response.RequestId);
        }

        [TestMethod]
        public void Response_Constructor_MarshalledData_FailureResponse()
        {
            var controlResponse = CreateControlEchoFailureResponse();
            var response = new Response(controlResponse.Payload);

            AssertFailureResponse(response);

            Assert.AreEqual(_rootExceptionValue, response.Exception.Message);
            Assert.AreEqual(controlResponse.Exception.Message, response.Exception.Message);
            Assert.AreEqual(controlResponse.CorrelationId, response.CorrelationId);
            Assert.AreEqual(controlResponse.DestinationPath, response.DestinationPath);
            Assert.AreEqual(controlResponse.TimeStamp, response.TimeStamp);
            Assert.AreEqual(controlResponse.Id, response.Id);
            Assert.AreEqual(controlResponse.RequestId, response.RequestId);
        }
    }
}
