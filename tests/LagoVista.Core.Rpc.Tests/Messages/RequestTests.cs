using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Rpc.Tests.Messages
{
    [TestClass]
    public class RequestTests
    {
        private readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        private readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        private readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        private readonly string _echoMethodParamName = "value";

        private IRequest CreateControlEchoRequest()
        {
            return new Request(_echoMethodInfo, _echoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }

        [TestMethod]
        public void Request_Constructor_MethodInfo_Args()
        {
            var request = new Request(_echoMethodInfo, _echoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Request_Constructor_MethodInfo_NullArgument()
        {
            MethodInfo echoMethodInfo = null;
            var request = new Request(echoMethodInfo, _echoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }

        [TestMethod]
        public void Request_Constructor_MarshalledData()
        {
            var controlRequest = CreateControlEchoRequest();

            var request = new Request(controlRequest.Payload);

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
        public void Request_Constructor_MethodInfo_SimpleArg()
        {
            var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new Request(echoMethodInfo, _echoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Request_Constructor_MethodInfo_ParamsAr_ThrowsNotSupportedException()
        {
            var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.PassStringParams));
            var request = new Request(echoMethodInfo, _echoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(_echoMethodParamName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToCountMismatch()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new Request(methodInfo, new object[] { ProxySubject.EchoValueConst, new object(), null }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToTypeMismatch()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var request = new Request(methodInfo, new object[] { 3 }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }
    }
}
