using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityGraph : EntityHeader
    {
        public List<EntityGraph> Children { get; } = new List<EntityGraph>();
        public string HostEntityName { get; set; }
        public string HostEntityType { get; set; }
        public string HostEntityId { get; set; }
        public bool IsHostEntityPublic { get; set; }
        public bool Exists { get; set; } = true;
    }
}
