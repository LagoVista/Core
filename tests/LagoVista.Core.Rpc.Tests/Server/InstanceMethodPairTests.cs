// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4a9f55173fd28047703c7ba9f8f434c17b80973fbeeae62c076e1356acb4ae3f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class InstanceMethodPairTests
    {

        #region synchronous method simple params
        [TestMethod]
        public void InstanceMethodPair_Constructor_SynchronousMethodAndSimpleMethodParams()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            Assert.IsNotNull(pair);
        }

        [TestMethod]
        public async Task InstanceMethodPair_Invoke_SynchronousMethodAndSimpleMethodParams_ReturnsNonNullResponse()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));

            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            var response = await pair.InvokeAsync(request);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task InstanceMethodPair_Invoke_SynchronousMethodAndSimpleMethodParams_ReturnsSuccessfulResponse()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));

            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            var response = await pair.InvokeAsync(request);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task InstanceMethodPair_Invoke_SynchronousMethodAndSimpleMethodParams_ReturnsCorrectResponse()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));

            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            var response = await pair.InvokeAsync(request);
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(request.Id, response.RequestId);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.ReturnValue);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.GetTypedReturnValue<string>());
        }
        #endregion

        #region asynchronous method simple params
        [TestMethod]
        public void InstanceMethodPair_Constructor_AsynchronousMethodAndSimpleMethodParams()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.EchoAsync));
            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            Assert.IsNotNull(pair);
        }

        [TestMethod]
        public async Task InstanceMethodPair_Invoke_AsynchronousMethodAndSimpleMethodParams_ReturnsNonNullResponse()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.EchoAsync));

            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            var response = await pair.InvokeAsync(request);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task InstanceMethodPair_Invoke_AsynchronousMethodAndSimpleMethodParams_ReturnsSuccessfulResponse()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.EchoAsync));

            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            var response = await pair.InvokeAsync(request);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task InstanceMethodPair_Invoke_AsynchronousMethodAndSimpleMethodParams_ReturnsCorrectResponse()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.EchoAsync));

            var subject = (IProxySubject)new ProxySubject();
            var pair = new InstanceMethodPair(subject, methodInfo);
            var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            var response = await pair.InvokeAsync(request);
            Assert.AreEqual(request.CorrelationId, response.CorrelationId);
            Assert.AreEqual(request.Id, response.RequestId);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.ReturnValue);
            Assert.AreEqual(ProxySubject.EchoValueConst, response.GetTypedReturnValue<string>());
        }
        #endregion

        #region invoke null param check
        [TestMethod]
        public async Task InstanceMethodPair_Invoke_SynchronousMethodAndSimpleMethodParams_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                var subject = (IProxySubject)new ProxySubject();
                var pair = new InstanceMethodPair(subject, methodInfo);
                IRequest request = null;

                var response = await pair.InvokeAsync(request);
            });
        }
        #endregion

        #region arguments validation
        [TestMethod]
        public void InstanceMethodPair_GetArguments_Succeeds_ReturnsNonNullArguments()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var parameters = methodInfo.GetParameters();
            var request = new Request(methodInfo, new object[] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            Assert.AreEqual(1, request.ArgumentCount);
            Assert.AreEqual(1, parameters.Length);

            var arguments = InstanceMethodPair.GetArguments(request, parameters);
            Assert.IsNotNull(arguments);
        }

        [TestMethod]
        public void InstanceMethodPair_GetArguments_Succeeds_ReturnsCorrectArguments()
        {
            var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
            var parameters = methodInfo.GetParameters();
            var requestArguments = new object[] { ProxySubject.EchoValueConst };
            var request = new Request(methodInfo, requestArguments, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

            Assert.AreEqual(1, request.ArgumentCount);
            Assert.AreEqual(1, parameters.Length);

            var arguments = InstanceMethodPair.GetArguments(request, parameters);

            Assert.AreEqual(parameters.Length, arguments.Length);
            Assert.AreEqual(request.ArgumentCount, arguments.Length);
            Assert.IsTrue(requestArguments.SequenceEqual(arguments));
        }

        [TestMethod]
        public void InstanceMethodPair_GetArguments_Fails_DueToCountMismatch()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                var parameters = methodInfo.GetParameters();
                var request = new Request(methodInfo, new object[] { ProxySubject.EchoValueConst, new object(), null }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

                Assert.AreEqual(3, request.ArgumentCount);
                Assert.AreEqual(1, parameters.Length);

                var arguments = InstanceMethodPair.GetArguments(request, parameters);
            });
        }

        [TestMethod]
        public void InstanceMethodPair_GetArguments_Fails_DueToTypeMismatch()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                var parameters = methodInfo.GetParameters();
                var request = new Request(methodInfo, new object[] { 3 }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

                Assert.AreEqual(1, request.ArgumentCount);
                Assert.AreEqual(1, parameters.Length);

                var arguments = InstanceMethodPair.GetArguments(request, parameters);
            });
        }

        [TestMethod]
        public void InstanceMethodPair_GetArguments_Fails_DueToUnsupportedParamsKeyword()
        {
            Assert.ThrowsExactly<NotSupportedException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.PassStringParams));
                var parameters = methodInfo.GetParameters();
                var request = new Request(methodInfo, new object[] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

                Assert.AreEqual(1, request.ArgumentCount);
                Assert.AreEqual(1, parameters.Length);

                var arguments = InstanceMethodPair.GetArguments(request, parameters);
            });
        }

        [TestMethod]
        public void InstanceMethodPair_GetArguments_NullRequest_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                var parameters = methodInfo.GetParameters();
                var subject = (IProxySubject)new ProxySubject();
                var pair = new InstanceMethodPair(subject, methodInfo);
                IRequest request = null;

                var arguments = InstanceMethodPair.GetArguments(request, parameters);
            });
        }

        [TestMethod]
        public void InstanceMethodPair_GetArguments_NullParameters_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var methodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
                ParameterInfo[] parameters = null;
                var subject = (IProxySubject)new ProxySubject();
                var pair = new InstanceMethodPair(subject, methodInfo);
                var request = new Request(methodInfo, new object[1] { ProxySubject.EchoValueConst }, Constants.OrganizationId, Constants.InstanceId, Constants.MessageReplyPath);

                var arguments = InstanceMethodPair.GetArguments(request, parameters);
            });
        }

        #endregion
    }
}

