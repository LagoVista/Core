using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client.Interfaces;
using LagoVista.Core.Rpc.Client.Transports;
using LagoVista.Core.Rpc.Settings;
using LagoVista.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core.Rpc.Client
{
    public static class RpcModule
    {
        public static void AddRpcModule(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddRcgClientModule();
            services.AddTransient<IProxyFactory, ProxyFactory>();
            services.AddSingleton<IRpcInvocationTransport, RemoteControlRpcInvocationTransport>();
            services.AddSingleton<ITransceiverConnectionSettings, TransceiverConnectionSettings>();
        }
    }
}
