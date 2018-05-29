namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequest : IAsyncMessage
    {
        int ArgumentCount { get; }
    }
}
