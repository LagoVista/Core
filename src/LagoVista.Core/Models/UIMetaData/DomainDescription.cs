// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9746ffa951ee27485cd2d0bc645f8d7b96991ad0f14563104a8c81e4e0df6cd9
// IndexVersion: 1
// --- END CODE INDEX META ---
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
        public string SourceAssembly { get; set; }

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
