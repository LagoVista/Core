using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Managers;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Rcg.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core.Rcg.Client
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRcgGatewayHttpClient, RcgGatewayHttpClient>();
            services.AddScoped<IRcgRpcClientTransport, RcgRpcClientTransport>();
            services.AddScoped<IRcgStatusManager, RcgStatusManager>();
            services.AddTransient<IRcgClientSettings, RcgClientSettings>();
        }
    }
}

namespace LagoVista.DependencyInjection
{
    public static class RcgClientModule
    {
        public static void AddRcgClientModule(this IServiceCollection services)
        {
            LagoVista.Core.Rcg.Client.Startup.ConfigureServices(services);
        }
    }
}
