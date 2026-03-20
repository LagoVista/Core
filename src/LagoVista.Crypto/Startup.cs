using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.Crypto;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Services.Crypto;
using LagoVista.Crypto.Modern;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Crypto
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAadBuilder, AadBuilderV1>();
            services.AddSingleton<IEnvelopeCodec, EnvelopeCodecV2>();
            services.AddSingleton<IKeyMaterialStore, SecureStorageKeyMaterialStore>();
            services.AddSingleton<IAeadEncryptor, AesGcmEncryptorNet9>();
            services.AddSingleton<IModernKeyIdBuilder, ModernKeyIdBuilder>();
            services.AddSingleton<IModernEncryption, ModernEncryptionService>();
        }
    }
}

namespace LagoVista.DependencyInjection
{
    public static class BillingModule
    {
        public static void AddModernCrypto(this IServiceCollection services, ILogger logger)
        {
            LagoVista.Crypto.Startup.ConfigureServices(services);
        }
    }
}