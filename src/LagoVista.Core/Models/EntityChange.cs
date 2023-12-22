using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityChangeSet
    {
        public EntityHeader ChangedBy {get; set;}
        public string ChangeDate { get; set; }
        public List<EntityChange> Changes { get; set; } = new List<EntityChange>();
    }

    public class EntityChange
    {
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Notes { get; set; }
    }
}
