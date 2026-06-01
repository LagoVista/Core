using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Security;
using LagoVista.Core.Services;
using LagoVista.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

            services.TryAddSingleton<ISummaryListProviderRegistry>(SummaryListProviderRegistry.Instance);
            services.TryAddTransient<ISummaryListProviderInvoker, SummaryListProviderInvoker>();
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

        public static void RegisterSummaryListProviders<TMarkerType>(this IServiceCollection services)
        {
            SummaryListProviderRegistry.Instance.RegisterAssembly(typeof(TMarkerType).Assembly, MetaDataHelper.Instance);
        }
    }
}