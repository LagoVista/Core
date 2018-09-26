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
