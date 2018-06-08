namespace LagoVista.Core.Rpc.Messages
{
    public interface IRequest : IMessage
    {
        int ArgumentCount { get; }
    }
}
