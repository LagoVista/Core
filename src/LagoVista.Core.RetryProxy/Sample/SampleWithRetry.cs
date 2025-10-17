// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 829532e70fa94d7900c598b9c5964dafa6b8a7a98007945c09cadadfe01ac7f1
// IndexVersion: 1
// --- END CODE INDEX META ---
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
