using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Retry.Sample
{
    internal class SampleWithRetry: ISample 
    {
        private readonly ISample _sampleProxy;

        public SampleWithRetry(RetryOptions retryOptions)
        {
            _sampleProxy = RetryProxy.Create<ISample, Sample>(retryOptions, new SampleRetryTester());
        }

        public string Echo(string value)
        {
            return _sampleProxy.Echo(value);
        }

        public Task<string> EchoAsync(string value)
        {
            return _sampleProxy.EchoAsync(value);
        }

        public DateTime Ping()
        {
            return _sampleProxy.Ping();
        }

        public void ThrowNonRetriableException()
        {
            _sampleProxy.ThrowNonRetriableException();
        }

        public void ThrowRetriableException()
        {
            _sampleProxy.ThrowRetriableException();
        }
    }
}
