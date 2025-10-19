// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a1c996c77bf958a99581c911463d64990e27399090cc8441e3bc8614ca2785d9
// IndexVersion: 0
// --- END CODE INDEX META ---
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
