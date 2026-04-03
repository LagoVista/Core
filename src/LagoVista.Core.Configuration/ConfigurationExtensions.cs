using LagoVista.Core.Configuration;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace LagoVista
{
    public static class ConfigurationExtensions
    {
        public static TConnection Set<TConnection>(this IConfiguration configuration, string sectionName, Action<IConfigurationSection, TConnection> configure) where TConnection : new()
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (String.IsNullOrEmpty(sectionName)) throw new ArgumentException(nameof(sectionName));

            var section = configuration.GetSection(sectionName);
            if (!section.Exists())
            {
                throw new InvalidOperationException($"Missing configuration section '{sectionName}'.");
            }

            var connection = new TConnection();
            configure(section, connection);
            return connection;
        }

        public static string Require(this IConfigurationSection section, string key)
        {
            if (section == null) throw new ArgumentNullException(nameof(section));
            if (String.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            var path = $"{section.Path}:{key}";

            var value = section[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                ConfigurationDiagnostics.AddRegisteredConfiguration(path, keyPresent: value != null, valuePresent: false);
            }
            else
                ConfigurationDiagnostics.AddRegisteredConfiguration(path);

            return value;
        }

        public static string Require(this IConfiguration section, string key)
        {
            if (section == null) throw new ArgumentNullException(nameof(section));
            if (String.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            var path = $"{key}";

            var value = section[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                ConfigurationDiagnostics.AddRegisteredConfiguration(path, keyPresent: value != null, valuePresent: false);
            }
            else
                ConfigurationDiagnostics.AddRegisteredConfiguration(path);

            return value;
        }

        public static string Optional(this IConfigurationSection section, string key, string fallback = null)
        {
            if (section == null) throw new ArgumentNullException(nameof(section));
            if (String.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            var path = $"{section.Path}:{key}";

            var value = section[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                ConfigurationDiagnostics.AddOptionalConfiguration(path, keyPresent: value != null, valuePresent: false);
                return fallback;
            }
            else
                ConfigurationDiagnostics.AddOptionalConfiguration(path);

            return value;
        }

        public static string Optional(this IConfiguration section, string key, string fallback = null)
        {
            if (section == null) throw new ArgumentNullException(nameof(section));
            if(String.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            var path = $"{key}";

            var value = section[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                ConfigurationDiagnostics.AddOptionalConfiguration(path, keyPresent: value != null, valuePresent: false);
                return fallback;
            }
            else
                ConfigurationDiagnostics.AddOptionalConfiguration(path);

            return value;
        }

        public static IConnectionSettings CreateDBStorageSettings(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);

            return new ConnectionSettings()
            {
                Uri = section.Require("Endpoint"),
                AccessKey = section.Require("AccessKey"),
                ResourceName = section.Require("DbName")
            };
        }

        public static IConnectionSettings GetRabbitMQSettings(this IConfiguration configuration, string sectionKey)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (String.IsNullOrEmpty(sectionKey)) throw new ArgumentException(nameof(sectionKey));


            var section = configuration.GetSection(sectionKey);

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
            var section = configuration.GetSection(sectionName);
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