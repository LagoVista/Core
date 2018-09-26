using System;

namespace LagoVista.Core.Retry.Sample
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class SampleRetryAllowedAttribute : Attribute { }

    internal enum ErrorCodes
    {
        // retry not allowed
        Unknown,
        RuntimeError,

        // retry allowed
        [SampleRetryAllowed]
        NetworkError
    }

    internal class SampleException : Exception
    {
        public SampleException()
        {
        }

        public SampleException(ErrorCodes errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public SampleException(ErrorCodes errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public ErrorCodes ErrorCode { get; } = ErrorCodes.Unknown;
    }
}
