// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8424dceed74ac8a247b696ea0650f92a95dfafc451fc4685594a006aad387cef
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Retry;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.Retry
{

    [TestClass]
    public class RetryProxyTests
    {
        private class RetryTestException : Exception
        {
            public RetryTestException()
            {
            }

            public RetryTestException(string message) : base(message)
            {
            }

            public RetryTestException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        private interface IRetryTester
        {
            string Succeed();
            string Fail();
        }

        private class RetryTester : IRetryTester
        {
            public string Succeed()
            {
                return "succeed";
            }

            public string Fail()
            {
                throw new RetryTestException("fail");
            }
        }

        [TestMethod]
        public void RetryProxy_Success()
        {
            var instance = new RetryTester();

            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(5)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Succeed();
            Assert.AreEqual("succeed", result);
        }

        [TestMethod]
        public void RetryProxy_Fail_TransientFunctionEvaluatesTrue()
        {
            Assert.ThrowsExactly<ExceededMaxAttemptsException>( () =>
            {
                var instance = new RetryTester();
                var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(1000)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
                retryProxy.Fail();
            });
        }

        [TestMethod]
        public void RetryProxy_Fail_TransientEnumerationEvaluatesTrue()
        {
            Assert.ThrowsExactly<ExceededMaxAttemptsException>( () =>
            {
                var instance = new RetryTester();
                var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(5)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
                retryProxy.Fail();
            });
        }

        [TestMethod]
        public void RetryProxy_Fail_NotTransientByFunction()
        {
            Assert.ThrowsExactly<RetryNotAllowedException>( () =>
            {
                var instance = new RetryTester();
                var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(5)), exceptionBlackList: new Type[] { typeof(RetryTestException) });
                retryProxy.Fail();
            });
        }

        [TestMethod]
        public void RetryProxy_Fail_NotTransientByEnumerable()
        {
            Assert.ThrowsExactly<RetryNotAllowedException>(() =>
            {
                var instance = new RetryTester();
                var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(60)), exceptionWhiteList: new Type[] { typeof(NullReferenceException) });
                retryProxy.Fail();
            });
        }

        [TestMethod]
        public void RetryProxy_Fail_ExceedMaxAttempts()
        {
            Assert.ThrowsExactly<ExceededMaxAttemptsException>(() =>
            {
                var instance = new RetryTester();
                var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(60)));
                retryProxy.Fail();
            });
        }

        [TestMethod]
        public void RetryProxy_Fail_ExceedMaxWaitTime()
        {
            Assert.ThrowsExactly<ExceededMaxWaitTimeException>(() =>
            {
                var instance = new RetryTester();
                var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(10000, TimeSpan.FromSeconds(1)));
                retryProxy.Fail();
            });
        }
    }
}
