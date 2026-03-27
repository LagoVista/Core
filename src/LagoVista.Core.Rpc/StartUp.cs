using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core.Rpc
{
    public static class RpcModule
    {
        public static void AddRpcModule(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton<ITransceiverConnectionSettings, TransceiverConnectionSettings>();
        }
    }
}
