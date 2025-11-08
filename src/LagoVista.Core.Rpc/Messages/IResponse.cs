// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: df185a828ac145d06013a1331500c4f0aac9c1a7d9b42dd3a7d12fcd21c01a1e
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Rpc.Messages
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
