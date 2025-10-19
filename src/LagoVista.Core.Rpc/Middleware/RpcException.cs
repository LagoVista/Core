// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d69b9eaf7d62fc1753e1039e3b57d0a7f53c6be527c0ba1342e4ffde786e9518
// IndexVersion: 0
// --- END CODE INDEX META ---
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
