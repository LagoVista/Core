using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        protected static readonly MethodInfo _echoMethodInfo = typeof(ProxySubject).GetMethod(nameof(ProxySubject.Echo));
        protected static readonly object[] _echoArgs = new object[1] { ProxySubject.EchoValueConst };
        protected static readonly string _echoMethodParamValue = ProxySubject.EchoValueConst;
        protected static readonly string _echoMethodParamName = "value";
        protected static readonly string _responseValue = "jello babies";
        protected static readonly string _rootExceptionValue = "boo";

        protected static IAsyncRequest CreateControlEchoRequest()
        {
            return new AsyncRequest(_echoMethodInfo, _echoArgs);
        }

        protected static IAsyncResponse CreateControlEchoSuccessResponse()
        {
            var request = CreateControlEchoRequest();
            return new AsyncResponse(request, _responseValue);
        }

        protected static IAsyncResponse CreateControlEchoFailureResponse()
        {
            var request = CreateControlEchoRequest();
            var ex = new Exception(_rootExceptionValue, new Exception("hoo"));
            return new AsyncResponse(request, ex);
        }

    }
}
