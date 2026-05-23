using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using LagoVista.Core.Rcg.Client;
using LagoVista.DependencyInjection;

namespace LagoVista.Core.Rpc
{
    public static class RpcModule
    {
        public static void AddRpcModule(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddRcgClientModule();
            services.AddSingleton<ITransceiverConnectionSettings, TransceiverConnectionSettings>();
        }
    }
}
