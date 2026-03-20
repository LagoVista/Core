using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Models;
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


namespace LagoVista.DependencyInjection
{
    public static class CoreModule
    {
        public static void AddCoreModule(this IServiceCollection services)
        {
            Startup.ConfigureServices(services);
            services.AddMetaDataHelper<AppUserDTO>();
        }
    }
}