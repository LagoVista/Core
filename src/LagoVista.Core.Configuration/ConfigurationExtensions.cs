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

        public static string Require(this IConfiguration section, string key)
        {
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(key);

            var value = section[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(
                    $"Missing required configuration value 'Root:{key}'.");

            return value;
        }


        public static IConnectionSettings CreateDBStorageSettings(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetRequiredSection(sectionName);

            return new ConnectionSettings()
            {
                Uri = section.Require("Endpoint"),
                AccessKey = section.Require("AccessKey"),
                ResourceName = section.Require("DbName")
            };
        }

        public static IConnectionSettings GetRabbitMQSettings(this IConfiguration configuration, string sectionKey)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionKey);

            var section = configuration.GetRequiredSection(sectionKey);

            return new ConnectionSettings
            {
                Uri = section.Require("Uri"),
                UserName = section.Require("UserId"),
                Password = section.Require("Password"),
                ResourceName = section.Require("Queue")
            };
        }

        public static IConnectionSettings CreateTableStorageSettings(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetRequiredSection(sectionName);
            return new ConnectionSettings()
            {
                AccountId = section.Require("Name"),
                AccessKey = section.Require("AccessKey"),
            };
        }

        public static IConnectionSettings CreateDefaultDBStorageSettings(this IConfiguration configuration)
        {
            return configuration.CreateDBStorageSettings("DefaultDocDBStorage");
        }

        public static IConnectionSettings CreateDefaultTableStorageSettings(this IConfiguration configuration)
        {
            return configuration.CreateTableStorageSettings("DefaultTableStorage");
        }
    }
}
