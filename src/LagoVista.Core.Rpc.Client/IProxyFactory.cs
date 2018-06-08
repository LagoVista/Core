namespace LagoVista.Core.Rpc.Client
{
    public interface IProxyFactory
    {
        TProxyInterface Create<TProxyInterface>(ProxySettings proxySettings) where TProxyInterface : class;
    }
}
