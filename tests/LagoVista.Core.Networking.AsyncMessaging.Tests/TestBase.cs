using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        protected static readonly MethodInfo echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        protected static readonly object[] echoArgs = new object[1] { ProxySubject.EchoValueConst };
        protected static readonly string echoMethodParamValue = "ping";
        protected static readonly string echoMethodParamName = "value";
        protected static readonly string responseValue = "jello babies";
        protected static readonly string rootExceptionValue = "boo";

        protected static IAsyncRequest CreateControlEchoRequest()
        {
            return new AsyncRequest(echoMethodInfo, echoArgs);
        }

        protected static IAsyncResponse CreateControlEchoSuccessResponse()
        {
            var request = CreateControlEchoRequest();
            return new AsyncResponse(request, responseValue);
        }

        protected static IAsyncResponse CreateControlEchoFailureResponse()
        {
            var request = CreateControlEchoRequest();
            var ex = new Exception(rootExceptionValue, new Exception("hoo"));
            return new AsyncResponse(request, ex);
        }

    }
}
