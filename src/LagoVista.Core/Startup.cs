using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            AutoMapper.Startup.ConfigureServices(services);
            services.AddSingleton<ICoreAppServices, CoreAppServices>();
            services.AddSingleton<IClock, LagoVistaClock>();
        }
    }
}