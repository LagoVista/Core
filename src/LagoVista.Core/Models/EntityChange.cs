// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 05502f397da5f26fd8b5264208046c4846fbfa3b7f4862e8fabb8a3b384af314
// IndexVersion: 0
// --- END CODE INDEX META ---
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
