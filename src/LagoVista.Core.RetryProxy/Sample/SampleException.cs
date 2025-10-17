// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2366c65d80fbfb5c9e1d06f183dc58c475be892ddaea8f2487bfedb41ddf0c47
// IndexVersion: 1
// --- END CODE INDEX META ---
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
