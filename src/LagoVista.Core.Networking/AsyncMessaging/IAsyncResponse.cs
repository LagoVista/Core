using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncResponse : IAsyncMessage
    {
        bool Success { get; }
        Exception Exception { get; }

        object ReturnValue { get; }
    }

    public interface IAsyncResponse<T> : IAsyncResponse
    {
        T TypedReturnValue { get; }
    }
}
