using LagoVista.Core.Interfaces;
using LagoVista.Crypto.Modern;

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

            services.AddSingleton<IModernEncryption, ModernEncryptionService>();
        }
    }
}
