using LagoVista.Core.Retry;
using System;
using Xunit;

namespace LagoVista.Core.Tests.Retry
{

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

        [Fact]
        public void RetryProxy_Success()
        {
            var instance = new RetryTester();

            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(5)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
            var result = retryProxy.Succeed();
            Assert.Equal("succeed", result);
        }

        [Fact]
        public void RetryProxy_Fail_TransientFunctionEvaluatesTrue()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(1000)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
            var exception = Record.Exception(() => retryProxy.Fail());
        }

        [Fact]
        public void RetryProxy_Fail_TransientEnumerationEvaluatesTrue()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(5)), exceptionWhiteList: new Type[] { typeof(RetryTestException) });
            var exception = Record.Exception(() => retryProxy.Fail());
            Assert.IsType<ExceededMaxAttemptsException>(exception);
        }

        [Fact]
        public void RetryProxy_Fail_NotTransientByFunction()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(5)), exceptionBlackList: new Type[] { typeof(RetryTestException) });
            var exception = Record.Exception(() => retryProxy.Fail());
            Assert.IsType<RetryNotAllowedException>(exception);
        }

        [Fact]
        public void RetryProxy_Fail_NotTransientByEnumerable()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(5, TimeSpan.FromSeconds(60)), exceptionWhiteList: new Type[] { typeof(NullReferenceException) });
            var exception = Record.Exception(() => retryProxy.Fail());
            Assert.IsType<RetryNotAllowedException>(exception);
            Assert.Equal(RetryNotAllowedReason.WhiteList, (exception as RetryNotAllowedException).Reason);            
        }

        [Fact]
        public void RetryProxy_Fail_ExceedMaxAttempts()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(1, TimeSpan.FromSeconds(60)));
            var exception = Record.Exception(() => retryProxy.Fail());
            Assert.IsType<ExceededMaxAttemptsException>(exception);
        }

        [Fact]
        public void RetryProxy_Fail_ExceedMaxWaitTime()
        {
            var instance = new RetryTester();
            var retryProxy = RetryProxy.Create<IRetryTester>(instance, new RetryOptions(10000, TimeSpan.FromSeconds(1)));
            var exception = Record.Exception(() => retryProxy.Fail());
            Assert.IsType<ExceededMaxWaitTimeException>(exception);
        }
    }
}
