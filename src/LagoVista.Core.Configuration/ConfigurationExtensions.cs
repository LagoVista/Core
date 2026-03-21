using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using Microsoft.Extensions.Configuration;
using System;

namespace LagoVista
{
    public static class ConfigurationExtensions
    {
        public static TConnection Set<TConnection>(this IConfiguration configuration, string sectionName, Action<IConfigurationSection, TConnection> configure) where TConnection : new()
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionName);
            ArgumentNullException.ThrowIfNull(configure);

            var section = configuration.GetSection(sectionName);
            if (!section.Exists())
                throw new InvalidOperationException($"Missing configuration section '{sectionName}'.");

            var connection = new TConnection();
            configure(section, connection);
            return connection;
        }

        public static string Require(this IConfigurationSection section, string key)
        {
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(key);

            var value = section[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(
                    $"Missing required configuration value '{section.Path}:{key}'.");

            return value;
        }

        public static IConnectionSettings CreateDBStorageSettings(this IConfigurationRoot configuration, string sectionName, ILogger logger)
        {
            var section = configuration.GetRequiredSection(sectionName);

            return new ConnectionSettings()
            {
                Uri = section.Require("Endpoint"),
                AccessKey = section.Require("AccessKey"),
                ResourceName = section.Require("DbName")
            };
        }

        public static IConnectionSettings CreateTableStorageSettings(this IConfigurationRoot configuration, string sectionName, ILogger logger)
        {
            var section = configuration.GetRequiredSection(sectionName);
            return new ConnectionSettings()
            {
                AccountId = section.Require("Name"),
                AccessKey = section.Require("AccessKey"),
            };
        }

        public static IConnectionSettings CreateDefaultDBStorageSettings(this IConfigurationRoot configuration, ILogger logger)
        {
            return configuration.CreateDBStorageSettings("DefaultDocDBStorage", logger);
        }

        public static IConnectionSettings CreateDefaultTableStorageSettings(this IConfigurationRoot configuration, ILogger logger)
        {
            return configuration.CreateTableStorageSettings("DefaultTableStorage", logger);
        }
    }
}
