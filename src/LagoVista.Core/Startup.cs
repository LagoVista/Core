using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Security;
using LagoVista.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.Core
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            AutoMapper.Startup.ConfigureServices(services);
            services.AddScoped<ICoreAppServices, CoreAppServices>();
            services.AddSingleton<IClock, LagoVistaClock>();
            services.AddSingleton<IEntityTypeResolver>(MetaDataHelper.Instance);
            services.AddSingleton<SignedRequestPublicKeySetResolver>(sp => new SignedRequestPublicKeySetResolver(new SignedRequestPublicKeySet()));
            
            services.AddSingleton<ISignedRequestPublicKeySetStore>(sp => sp.GetRequiredService<SignedRequestPublicKeySetResolver>());
            services.AddSingleton<ISignedRequestValidationKeyResolver>(sp => sp.GetRequiredService<SignedRequestPublicKeySetResolver>());
            services.AddSingleton<ISignedRequestPublicKeySetClient, SignedRequestPublicKeySetHttpClient>();
            services.AddSingleton<ISignedRequestPublicKeyRefreshService, SignedRequestPublicKeyRefreshService>();
            services.AddSingleton<ISignedRequestValidatorService, SignedRequestValidatorService>();
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