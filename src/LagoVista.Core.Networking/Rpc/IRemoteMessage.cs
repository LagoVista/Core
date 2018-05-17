namespace LagoVista.Core.Networking.Rpc
{
    public interface IRemoteMessage
    {
        string Id { get; set; }
        string CorrelationId { get; set; }
        string Name { get; set; }
        byte[] MarshalledData { get; set; }
    }
}
