// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9dc6373f831b7b4e693d53055036e2d9acd10fc00629649f42321a2be41220fa
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Models.UIMetaData
{
    public class MetaDataHelper
    {
        static MetaDataHelper _instance = new MetaDataHelper();
        public static MetaDataHelper Instance { get { return _instance; } }

        private List<Assembly> _assemblies = new List<Assembly>();
        private Dictionary<string, DomainDescription> _domains = new Dictionary<string, DomainDescription>();

        private List<EntityDescription> _entities = new List<EntityDescription>();
        private List<EntitySummary> _entitySummaries = new List<EntitySummary>();

        public void RegisterAssembly(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                var attr = type.GetCustomAttributes<DomainDescriptorAttribute>().FirstOrDefault();

                foreach (var property in type.DeclaredProperties)
                {
                    if (property.PropertyType == typeof(DomainDescription))
                    {
                        var attrDomainDescription = property.GetCustomAttribute<DomainDescriptionAttribute>();
                        if (attrDomainDescription == null)
                            throw new NullReferenceException($"Could not find [DomainDescription] attribute on type {type.Name}.");

                        var domainDescription = property.GetValue(null, null) as DomainDescription;
                        if(domainDescription == null)
                            throw new NullReferenceException($"Could not get property value [DomainDescription]  on type {type.Name}.");

                        domainDescription.Key = attrDomainDescription.Key;
                        domainDescription.SourceAssembly = type.Assembly.FullName;

                        _domains.Add(attrDomainDescription.Key, domainDescription);
                    }
                }
            }

            foreach (var type in assembly.DefinedTypes)
            {
                var attr = type.GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    var entityDescription = EntityDescription.Create(type.AsType(), attr);
                    if (!_domains.ContainsKey(entityDescription.DomainName))
                        throw new NullReferenceException($"Could not find {entityDescription.DomainName} domain from {type.FullName}");

                    entityDescription.Domain = _domains[entityDescription.DomainName];
                  
                    _entities.Add(entityDescription);
                    var summary = EntitySummary.Create(entityDescription);
                    summary.ShortClassName = type.Name;
                    _entitySummaries.Add(summary);
                }
            }
            _assemblies.Add(assembly);
        }

        public List<DomainDescription> Domains { get { return _domains.Values.ToList(); } }

        public List<EntityDescription> Entities { get { return _entities; } }

        public List<EntitySummary> EntitySummaries { get { return _entitySummaries; } }
    }
}
