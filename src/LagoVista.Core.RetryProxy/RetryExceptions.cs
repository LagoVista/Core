// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5165dea4e622da2468b045e21bfb70667c534259c18df237b0702bc201459344
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Retry
{
    public class RetryException : Exception, IRetryProxyException
    {
        public RetryException()
        {
        }

        public RetryException(string message) : base(message)
        {
        }

        public RetryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int Attempts { get; internal set; }
        public TimeSpan Duration { get; internal set; }
        public List<Exception> Exceptions { get; internal set; }
    }

    public enum RetryNotAllowedReason
    {
        WhiteList,
        BlackList,
        RetryTester
    }

    public sealed class RetryNotAllowedException : RetryException
    {
        public RetryNotAllowedException(RetryNotAllowedReason reason) : base()
        {
            Reason = reason;
        }

        public RetryNotAllowedException(RetryNotAllowedReason reason, string message) : base(message)
        {
            Reason = reason;
        }

        public RetryNotAllowedException(RetryNotAllowedReason reason, string message, Exception innerException) : base(message, innerException)
        {
            Reason = reason;
        }

        public RetryNotAllowedReason Reason { get; }
    }

    public sealed class ExceededMaxAttemptsException : RetryException
    {
        public ExceededMaxAttemptsException() : base()
        {
        }

        public ExceededMaxAttemptsException(string message) : base(message)
        {
        }

        public ExceededMaxAttemptsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public sealed class ExceededMaxWaitTimeException : RetryException
    {
        public ExceededMaxWaitTimeException()
        {
        }

        public ExceededMaxWaitTimeException(string message) : base(message)
        {
        }

        public ExceededMaxWaitTimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
