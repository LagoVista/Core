using LagoVista.Core.Validation;
using System;

namespace LagoVista.Core.Rpc.Middleware
{
    public class RpcException : Exception
    {
        public RpcException() : base()
        {
        }

        public RpcException(string message) : base(message)
        {
        }

        public RpcException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static string FormatErrorMessage(ErrorMessage error, string prefix)
        {
            return $"{prefix} {error.ErrorCode}, {error.Message}, system error ? {error.SystemError}, {error.Details}";
        }
    }
}
