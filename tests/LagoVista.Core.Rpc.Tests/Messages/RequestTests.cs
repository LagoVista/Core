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
        public static IRequest CreateControlEchoRequest()
        {
            return new Request(Constants.EchoMethodInfo, Constants.EchoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }

        public static IRequest CreateControlVoidTaskRequest()
        {
            return new Request(Constants.VoidTaskMethodInfo, null, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }

        public static IRequest CreateControlVoidRequest()
        {
            return new Request(Constants.VoidMethodInfo, null, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
        }

        [TestMethod]
        public void Request_Constructor_MethodInfo_Args()
        {
            var request = new Request(Constants.EchoMethodInfo, Constants.EchoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(Constants.EchoMethodParamName));
            Assert.AreEqual(1, request.ArgumentCount);
        }

        [TestMethod]
        public void Request_Constructor_MethodInfo_NullArgument()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                MethodInfo echoMethodInfo = null;
                var request = new Request(echoMethodInfo, Constants.EchoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
            });
        }

        [TestMethod]
        public void Request_Constructor_MarshalledData()
        {
            var controlRequest = CreateControlEchoRequest();

            var request = new Request(controlRequest.Payload);

            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(Constants.EchoMethodParamName));
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
            var request = new Request(echoMethodInfo, Constants.EchoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
            Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(Constants.EchoMethodParamName));
        }

        [TestMethod]
        public void Request_Constructor_MethodInfo_ParamsAr_ThrowsNotSupportedException()
        {
            Assert.ThrowsExactly<NotSupportedException>(() =>
            {
                var echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.PassStringParams));
                var request = new Request(echoMethodInfo, Constants.EchoArgs, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
                Assert.AreEqual(ProxySubject.EchoValueConst, request.GetValue(Constants.EchoMethodParamName));
            });
        }

        [TestMethod]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToCountMismatch()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                var request = new Request(methodInfo, new object[] { ProxySubject.EchoValueConst, new object(), null }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
            });
        }

        [TestMethod]
        public void InstanceMethodPair_ValidateArguments_Fails_DueToTypeMismatch()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                var request = new Request(methodInfo, new object[] { 3 }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);
            });
        }
    }
}
