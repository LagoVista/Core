// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3f24380d134b33dea71410451d724876a53eb7edb7c41abfb9dcc6e9874a9307
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Retry
{
    public interface IRetryProxyException
    {
        int Attempts { get; }
        TimeSpan Duration { get; }
        List<Exception> Exceptions { get; }
    }
}
