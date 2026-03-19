using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core.AutoMapper
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            ConvertersRegistration.ConfigureServices(services);

            services.AddSingleton<IEncryptedMapper, EncryptedMapper>();
            services.AddSingleton<IEncryptor, Encryptor>();
            services.AddSingleton<IAtomicPlanBuilder, ReflectionAtomicPlanBuilder>();
            services.AddScoped<IEncryptionKeyProvider, EncryptionKeyProvider>();
            services.AddSingleton<ILagoVistaAutoMapper, LagoVistaAutoMapper>();
            services.AddSingleton<IEncryptedMapperPlanner, EncryptedMapperPlanner>();
        }
    }
}
