// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 39cca569fd556d178417e86909df1e4977581bc9ce0a4f7d824de77ecd505d5f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DomainDescriptionAttribute : Attribute
    {
        private String _key;

        public DomainDescriptionAttribute(String Key)
        {
            _key = Key;
        }

        public String Key { get { return _key; } }
    }
}
