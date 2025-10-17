// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 01a22f142e026597366c83e38affe7d69cdfa1c51fc0f0a1639dbfbd9b1b9186
// IndexVersion: 1
// --- END CODE INDEX META ---
namespace LagoVista.Core.Rpc.Client
{
    public interface IProxyFactory
    {
        TProxyInterface Create<TProxyInterface>(IProxySettings proxySettings) where TProxyInterface : class;
    }
}
