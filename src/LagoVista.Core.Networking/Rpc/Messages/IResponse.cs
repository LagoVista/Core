using System;

namespace LagoVista.Core.Networking.Rpc.Messages
{
    public interface IResponse : IMessage
    {
        bool Success { get; }
        Exception Exception { get; }
        object ReturnValue { get; }
        string RequestId { get; }
        T GetTypedReturnValue<T>();
    }
}
