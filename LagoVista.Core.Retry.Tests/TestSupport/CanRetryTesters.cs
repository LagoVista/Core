using System;

namespace LagoVista.Core.Retry
{
    public class CanRetryProxySubjectException : IRetryExceptionTester
    {
        public bool CanRetry(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            return exception is ProxySubjectTestException;
        }
    }

    public class CanNotRetryProxySubjectException : IRetryExceptionTester
    {
        public bool CanRetry(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            return !(exception is ProxySubjectTestException);
        }
    }
}
