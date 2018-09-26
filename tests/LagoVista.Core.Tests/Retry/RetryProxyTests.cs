using LagoVista.Core.Retry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        [ExpectedException(typeof(ExceededMaxAttemptsException))]
        public void RetryProxy_Fail_TransientFunctionEvaluatesTrue()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(1000)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxAttemptsException))]
        public void RetryProxy_Fail_TransientEnumerationEvaluatesTrue()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(5)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(RetryNotAllowedException))]
        public void RetryProxy_Fail_NotTransientByFunction()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(5)), exceptionBlackList: new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(RetryNotAllowedException))]
        public void RetryProxy_Fail_NotTransientByEnumerable()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(60)), exceptionWhiteList: new Type[] { typeof(NullReferenceException) });
            try
            {
                var result = retryProxy.Fail();
            }
            catch (RetryNotAllowedException ex)
            {
                Assert.AreEqual(RetryNotAllowedReason.WhiteList, ex.Reason);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxAttemptsException))]
        public void RetryProxy_Fail_ExceedMaxAttempts()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(60)));
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxWaitTimeException))]
        public void RetryProxy_Fail_ExceedMaxWaitTime()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(10000, TimeSpan.FromSeconds(1)));
            var result = retryProxy.Fail();
        }
    }
}
