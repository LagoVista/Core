using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncMessage
    {
        string Id { get; set; }
        string CorrelationId { get; set; }
        string Path { get; set; }
        DateTime TimeStamp { get; set; }

        byte[] MarshalledData { get; set; }
    }
}
