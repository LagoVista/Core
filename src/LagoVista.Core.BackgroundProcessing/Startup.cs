using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core.BackgroundProcessing
{
    public class Startup
    {
        public static void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddHostedService<BackgroundTaskProcessor>();
        }
    }
}
