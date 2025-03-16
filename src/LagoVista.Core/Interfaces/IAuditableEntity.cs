using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{

    public interface IAuditableEntitySimple
    {
        String CreationDate { get; set; }
        String LastUpdatedDate { get; set; }
        EntityHeader CreatedBy { get; set; }
        EntityHeader LastUpdatedBy { get; set; }

        bool? IsDeleted { get; set; }
        EntityHeader DeletedBy { get; set; }
        String DeletionDate { get; set; }
    }

    public interface IAuditableEntity : IAuditableEntitySimple
    {
        List<EntityChangeSet> AuditHistory { get; set; }

        bool IsDeprecated { get; set; }
        EntityHeader DeprecatedBy { get; set; }
        String DeprecationDate { get; set; }
        String DeprecationNotes { get; set; }
    }
}
