using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncResponse : IAsyncMessage
    {
        bool Success { get; }
        Exception Exception { get; }
        object ReturnValue { get; }
        string RequestId { get; }
        T GetTypedReturnValue<T>();
    }
}
