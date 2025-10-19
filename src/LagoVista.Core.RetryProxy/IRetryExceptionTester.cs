// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bafc9f6b6b2b781d50615c0b6a1c1555943d3ff5ebfac7b0349aaa5e5407dfe7
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Retry
{
    public interface IRetryExceptionTester
    {
        bool CanRetry(Exception exception);
    }
}
