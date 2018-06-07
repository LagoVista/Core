using System;

namespace LagoVista.Core.Networking.Rpc.Middleware
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
    }
}
