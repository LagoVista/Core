using LagoVista.Core.Interfaces;
using LagoVista.Crypto.Modern;

namespace LagoVista.Crypto
{
    public static class Startup
    {
        /// <summary>
        /// Registers the modern encryption services.
        /// Note: ModernEncryptionService requires EntityHeader org + user to be registered/provided by the host.
        /// </summary>
        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IAadBuilder, AadBuilderV1>();
            services.AddSingleton<IEnvelopeCodec, EnvelopeCodecV2>();
            services.AddSingleton<IKeyMaterialStore, SecureStorageKeyMaterialStore>();
            services.AddSingleton<IAeadEncryptor, AesGcmEncryptorNet9>();

            services.AddSingleton<IModernEncryption, ModernEncryptionService>();
        }
    }
}
