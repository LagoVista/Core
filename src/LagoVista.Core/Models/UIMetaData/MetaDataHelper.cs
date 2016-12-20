using LagoVista.Core.Attributes;
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

        public void RegisterAssembly(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                var attr = type.GetCustomAttributes<DomainDescriptorAttribute>().FirstOrDefault();

                foreach(var property in type.DeclaredProperties)
                {
                    if(property.PropertyType == typeof(DomainDescription))
                    {
                        var attrDomainDescription = property.GetCustomAttribute<DomainDescriptionAttribute>();

                        var domainDescription = property.GetValue(null,null) as DomainDescription;
                        domainDescription.Key = attrDomainDescription.Key;
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
                    entityDescription.Domain = _domains[entityDescription.DomainName];
                }
            }
            _assemblies.Add(assembly);
        }

         public List<DomainDescription> Domains { get { return _domains.Values.ToList(); } }
    }
}
