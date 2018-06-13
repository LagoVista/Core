using System;

namespace LagoVista.Core.Retry
{
    public class RetryException : Exception
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
    }

    public sealed class NotTransientException : RetryException
    {
        public NotTransientException()
        {
        }

        public NotTransientException(string message) : base(message)
        {
        }

        public NotTransientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public sealed class ExceededMaxAttemptsException : RetryException
    {
        public ExceededMaxAttemptsException()
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
