using System;

namespace LagoVista.Core.Networking.Rpc
{
    public interface IRemoteResponse : IRemoteMessage
    {
        bool Success { get; }
        Exception Exception { get; }
        object Response { get; set; }
    }

    public interface IRemoteResponse<T> : IRemoteResponse
    {
        T TypedResponse { get; }
    }
}
