using System;

namespace LagoVista.Core.Retry
{
    public interface IRetryExceptionTester
    {
        bool CanRetry(Exception exception);
    }
}
