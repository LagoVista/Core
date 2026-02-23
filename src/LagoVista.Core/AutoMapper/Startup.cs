using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;

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
            services.AddSingleton<IEncryptionKeyProvider, EncryptionKeyProvider>();
            services.AddSingleton<ILagoVistaAutoMapper, LagoVistaAutoMapper>();
        }
    }
}
