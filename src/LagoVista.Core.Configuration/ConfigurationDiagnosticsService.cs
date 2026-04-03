using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Configuration
{
    public interface IConfigurationDiagnosticsService
    {
        void Populate();
        IReadOnlyList<ConfigurationDiagnosticContext> GetDiagnostics();
        void ThrowIfMissingRequired();
        IReadOnlyList<ConfigurationDiagnosticContext> Errors { get; }
        List<string> Exceptions { get; }
        IReadOnlyList<ConfigurationDiagnosticContext> Warnings { get; }
        bool IsValid { get; }
    }

    public sealed class ConfigurationDiagnosticsService : IConfigurationDiagnosticsService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceRegistrationSnapshot _snapshot;
   
        public ConfigurationDiagnosticsService(IServiceProvider serviceProvider, ServiceRegistrationSnapshot snapshot)
        {
            _serviceProvider = serviceProvider;
            _snapshot = snapshot;
        }   

        public void Populate()
        {
            foreach (var entry in _snapshot.Entries)
            {
                var implementationType = entry.ImplementationType;
                if (implementationType == null)
                {
                    continue;
                }

                if (!implementationType.IsClass || implementationType.IsAbstract)
                {
                    continue;
                }

                var hasConfigurationCtor = implementationType
                    .GetConstructors()
                    .Any(ctor => ctor.GetParameters().Any(param =>
                        param.ParameterType == typeof(IConfiguration) ||
                        param.ParameterType == typeof(IConfigurationRoot)));

                if (!hasConfigurationCtor)
                {
                    continue;
                }

                try
                {
                    ConfigurationDiagnostics.CurrentClassName = implementationType.FullName ?? implementationType.Name;
                    _ = _serviceProvider.GetService(entry.ServiceType);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error while resolving {ConfigurationDiagnostics.CurrentClassName}: {ex.Message}");
                    Console.ResetColor();
                    Exceptions.Add($"Class: {ConfigurationDiagnostics.CurrentClassName} - {ex.Message}");
                    // first cut: swallow and let the diagnostics collector tell the story
                }
                finally
                {
                    ConfigurationDiagnostics.CurrentClassName = null;
                }
            }
        }

        public bool IsValid => !GetDiagnostics().Any(x => !x.Optional && !x.ValuePresent);
    
        public IReadOnlyList<ConfigurationDiagnosticContext> Errors => GetDiagnostics().Where(x => !x.Optional && !x.ValuePresent).ToList();
        public IReadOnlyList<ConfigurationDiagnosticContext> Warnings  => GetDiagnostics().Where(x => x.Optional && !x.ValuePresent).ToList();
        public List<string> Exceptions { get; } = new List<string>();   


        public IReadOnlyList<ConfigurationDiagnosticContext> GetDiagnostics()
        {
            return ConfigurationDiagnostics.GetEntries();
        }
        
        public void ThrowIfMissingRequired()
        {
            var missing = GetDiagnostics().Where(x => !x.Optional && !x.ValuePresent).ToList();
            if (missing.Any())
            {
                var missngKeys = String.Join('\t', missing.Select(x => $"\tClass={x.Class}; Path={x.Path}\r\n"));
                throw new InvalidConfigurationException($"Please add the following missing keys:{missngKeys}");
            }
        }
    }

    public sealed class ServiceRegistrationSnapshot
    {
        public List<ServiceRegistrationEntry> Entries { get; }

        public ServiceRegistrationSnapshot(IServiceCollection services)
        {
            Entries = services
                .Select(descriptor => new ServiceRegistrationEntry
                {
                    ServiceType = descriptor.ServiceType,
                    ImplementationType = descriptor.ImplementationType,
                    Lifetime = descriptor.Lifetime
                })
                .ToList();
        }
    }

    public sealed class ServiceRegistrationEntry
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public ServiceLifetime Lifetime { get; set; }
    }
}
