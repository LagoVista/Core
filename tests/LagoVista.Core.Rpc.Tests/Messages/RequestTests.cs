using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Rpc.Tests.Messages
{
    [TestClass]
    public class RequestTests
    {
        private readonly MethodInfo _echoMethodInfo = typeof(FakeProxySubject).GetMethod(nameof(FakeProxySubject.Echo));
        private readonly object[] _echoArgs = new object[1] { FakeProxySubject.EchoValueConst };
        private readonly string _echoMethodParamValue = FakeProxySubject.EchoValueConst;
        private readonly string _echoMethodParamName = "value";

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

        [TestMethod]
        public void Request_Constructor_MethodInfo_Args()
        {
            var request = new Request(_echoMethodInfo, _echoArgs, _organizationId, _instanceId, _replyPath);

            Assert.AreEqual(FakeProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Request_Constructor_MethodInfo_NullArgument()
        {
            MethodInfo echoMethodInfo = null;
            var request = new Request(echoMethodInfo, _echoArgs, _organizationId, _instanceId, _replyPath);
        }

        [TestMethod]
        public void Request_Constructor_MarshalledData()
        {
            var controlRequest = CreateControlEchoRequest();

            var request = new Request(controlRequest.Payload);

            Assert.AreEqual(FakeProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);

            Assert.AreEqual(controlRequest.Id, request.Id);
            Assert.AreEqual(controlRequest.CorrelationId, request.CorrelationId);
            Assert.AreEqual(controlRequest.DestinationPath, request.DestinationPath);
            Assert.AreEqual(controlRequest.TimeStamp, request.TimeStamp);
            Assert.AreEqual(controlRequest.Json, request.Json);
            Assert.IsTrue(controlRequest.Payload.SequenceEqual(request.Payload));
        }

        [TestMethod]
        public void Request_Constructor_MethodInfo_SimpleArg()
        {
            var echoMethodInfo = typeof(FakeProxySubject).GetMethod(nameof(FakeProxySubject.Echo));
            var request = new Request(echoMethodInfo, _echoArgs, _organizationId, _instanceId, _replyPath);
            Assert.AreEqual(FakeProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Request_Constructor_MethodInfo_ParamsAr_ThrowsNotSupportedException()
        {
            var echoMethodInfo = typeof(FakeProxySubject).GetMethod(nameof(FakeProxySubject.PassStringParams));
            var request = new Request(echoMethodInfo, _echoArgs, _organizationId, _instanceId, _replyPath);
            Assert.AreEqual(FakeProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToCountMismatch()
        {
            var methodInfo = typeof(FakeProxySubject).GetMethod(nameof(FakeProxySubject.Echo));
            var request = new Request(methodInfo, new object[] { FakeProxySubject.EchoValueConst, new object(), null }, _organizationId, _instanceId, _replyPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToTypeMismatch()
        {
            var methodInfo = typeof(FakeProxySubject).GetMethod(nameof(FakeProxySubject.Echo));
            var request = new Request(methodInfo, new object[] { 3 }, _organizationId, _instanceId, _replyPath);
        }
    }
}
