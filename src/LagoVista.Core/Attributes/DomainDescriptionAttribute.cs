using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DomainDescriptionAttribute : Attribute
    {
        private String _key;

        public DomainDescriptionAttribute(String Key)
        {
            _key = Key;
        }

        public String Key { get; private set; }
    }

    public class DomainDescription
    {
        public String Description { get; set; }
        public String Name { get; set; }
        public VersionInfo CurrentVersion { get; set; }
        public List<VersionInfo> VersionHistory { get; set; }

        public enum DomainTypes
        {
            Dto,
            BusinessObject,
            Storage,
            UserInterface
        }

        public DomainTypes DomainTyep { get; set; }

    }
}
