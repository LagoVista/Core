namespace LagoVista.Core.Networking.Rpc
{
    public interface IMessage
    {
        string Id { get; set; }
        string CorrelationId { get; set; }
        string Name { get; set; }
        string LockToken { get; set; }
        byte[] MarshalledData { get; set; }
    }
}
