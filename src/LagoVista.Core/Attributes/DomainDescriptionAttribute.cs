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
        private String _name;
        private String _description;
        private String _key;
        private VersionInfo _currentVersion;
        private List<VersionInfo> _versionHistory;

        public DomainDescriptionAttribute(String Name = "", String Description = "", String Key = "", VersionInfo currentVersion = null, List<VersionInfo> versionHistory = null)
        {
            _name = Name;
            _description = Description;
            _key = Key;
            _currentVersion = currentVersion;
            _versionHistory = versionHistory;
        }

        public String Name { get; private set; }
        public String Key { get; private set; }
        public String Description { get; private set; }
        public VersionInfo CurrentVersion { get; private set; }
        public List<VersionInfo> VersionHistory { get; private set; }
    }
}
