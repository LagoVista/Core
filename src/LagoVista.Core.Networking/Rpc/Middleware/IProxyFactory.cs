namespace LagoVista.Core.Networking.Rpc.Middleware
{
    public interface IProxyFactory
    {
        TProxyInterface Create<TProxyInterface>() where TProxyInterface : class;
    }
}
