using System;

namespace LagoVista.Core.Networking.Rpc
{
    public interface IResponse : IMessage
    {
        bool Success { get; }
        Exception Exception { get; }
        object Response { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        T TypedResponse { get; }
    }
}
