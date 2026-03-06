using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class RelationalEntityBase : IRelationalIDEntity
    {
        // This should NOT use our normalized Guid format, it's actually a GUID in the database.
        public GuidString36 Id { get; set; } = GuidString36.Factory();

        [MapTo(nameof(DbModelBase.Organization))]
        public EntityHeader OwnerOrganization { get; set; }
        
        [MapTo(nameof(DbModelBase.CreatedByUser))]
        public EntityHeader CreatedBy { get; set; }
        
        [MapTo(nameof(DbModelBase.LastUpdatedByUser))]
        public EntityHeader LastUpdatedBy { get; set; }
        public UtcTimestamp CreationDate { get; set; }
        [MapTo(nameof(DbModelBase.LastUpdatedDate))]
        public UtcTimestamp LastUpdatedDate { get; set; }
    }
}
