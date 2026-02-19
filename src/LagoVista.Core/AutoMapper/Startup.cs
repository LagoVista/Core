using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            ConvertersStartup.ConfigureServices(services);
            services.AddSingleton<IEncryptedMapper, EncryptedMapper>();
            services.AddSingleton<IEncryptor, Encryptor>();
            services.AddSingleton<IEncryptionKeyProvider, EncryptionKeyProvider>();
            services.AddSingleton<ILagoVistaAutoMapper, LagoVistaAutoMapper>();
        }
    }
}
