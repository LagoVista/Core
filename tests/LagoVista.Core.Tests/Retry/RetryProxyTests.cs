using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Retry;

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
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 5, MaxWaitTimeInSeconds = 5 }, new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Succeed();
            Assert.AreEqual("succeed", result);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxAttemptsException))]
        public void RetryProxy_Fail_TransientFunctionEvaluatesTrue()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 1, MaxWaitTimeInSeconds = 1000 }, (exception) => { return typeof(RetryTestException) == exception.GetType(); });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxAttemptsException))]
        public void RetryProxy_Fail_TransientEnumerationEvaluatesTrue()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 1, MaxWaitTimeInSeconds = 5 }, new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(NotTransientException))]
        public void RetryProxy_Fail_NotTransientByFunction()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 5, MaxWaitTimeInSeconds = 5 }, (exception) => { return false; });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(NotTransientException))]
        public void RetryProxy_Fail_NotTransientByEnumerable()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 5, MaxWaitTimeInSeconds = 5 }, new Type[] { typeof(NullReferenceException) });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxAttemptsException))]
        public void RetryProxy_Fail_ExceedMaxAttempts()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 1, MaxWaitTimeInSeconds = 60 });
            var result = retryProxy.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ExceededMaxWaitTimeException))]
        public void RetryProxy_Fail_ExceedMaxWaitTime()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions { MaxAttempts = 10000, MaxWaitTimeInSeconds = 1 });
            var result = retryProxy.Fail();
        }       
    }
}
