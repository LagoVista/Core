namespace LagoVista.Core.Networking.Rpc.Messages
{
    public interface IRequest : IMessage
    {
        int ArgumentCount { get; }
    }
}
