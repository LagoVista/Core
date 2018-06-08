namespace LagoVista.Core.Rpc.Client
{
    public interface IProxyFactory
    {
        TProxyInterface Create<TProxyInterface>(IProxySettings proxySettings) where TProxyInterface : class;
    }
}
