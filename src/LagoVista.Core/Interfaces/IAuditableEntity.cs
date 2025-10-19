// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a2d6d2bf8d2da46fb16e3d4213dc87f4c16d41afda5f6b73eb583f1b7d41d4f9
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;

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
        bool IsDraft { get; }
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
