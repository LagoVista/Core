using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.UIMetaData
{
    public class DomainDescription
    {
        public DomainDescription()
        {

        }

        public String Key { get; set; }

        public String Description { get; set; }
        public String Name { get; set; }
        public VersionInfo CurrentVersion { get; set; }
        public List<VersionInfo> VersionHistory { get; set; }

        public enum DomainTypes
        {
            Dto,
            BusinessObject,
            Storage,
            UserInterface,
            Service,
            Manager
        }

        public DomainTypes DomainType { get; set; }
    }
}
