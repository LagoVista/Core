using LagoVista.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LagoVista
{
    public static class RemoteConfigurationExtensions
    {
        public static IServiceCollection AddRemoteConfigurationClient(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IRemoteConfigurationClient, RemoteConfigurationClient>();

            return services;
        }
    }
}
