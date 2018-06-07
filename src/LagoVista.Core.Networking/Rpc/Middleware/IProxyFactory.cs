namespace LagoVista.Core.Networking.Rpc.Middleware
{
    public interface IProxyFactory
    {
        TProxyInterface Create<TProxyInterface>(string proxySettings) where TProxyInterface : class;
    }
}
