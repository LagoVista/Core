using LagoVista.Core.Configuration;
using LagoVista.Core.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista
{
    public static class RemoteConfigurationExtensions
    {
        public static IConfigurationRoot ToConfigurationRoot(this ResolvedConfiguration resolvedConfiguration)
        {
            if (resolvedConfiguration == null) throw new ArgumentNullException(nameof(resolvedConfiguration));
            if (resolvedConfiguration.Values == null) throw new InvalidOperationException("Resolved configuration does not contain a values collection.");

            var values = new Dictionary<string, string>(resolvedConfiguration.Values, StringComparer.OrdinalIgnoreCase);

            return new ConfigurationBuilder()
                .Add(new DictionaryConfigurationSource(values))
                .Build();
        }

        public static async Task<IConfigurationRoot> LoadRemoteConfigurationAsync(this IRemoteConfigurationClient client, RemoteConfigurationSettings settings, string appKey, string deploymentKey, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var resolvedConfiguration = await client.LoadAsync(settings, appKey, deploymentKey, cancellationToken).ConfigureAwait(false);
            return resolvedConfiguration.ToConfigurationRoot();
        }

        public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton(configuration.Map<T>());
            return services;
        }

        private sealed class DictionaryConfigurationSource : IConfigurationSource
        {
            private readonly IDictionary<string, string> _values;

            public DictionaryConfigurationSource(IDictionary<string, string> values)
            {
                _values = values ?? throw new ArgumentNullException(nameof(values));
            }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new DictionaryConfigurationProvider(_values);
            }
        }

        private sealed class DictionaryConfigurationProvider : ConfigurationProvider
        {
            public DictionaryConfigurationProvider(IDictionary<string, string> values)
            {
                Data = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
