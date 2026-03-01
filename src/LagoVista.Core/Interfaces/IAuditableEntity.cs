// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a2d6d2bf8d2da46fb16e3d4213dc87f4c16d41afda5f6b73eb583f1b7d41d4f9
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{

    public interface IAuditableEntitySimple : ISoftDeletable
    {
        String Sha256Hex { get; set; }
        UtcTimestamp CreationDate { get; set; }
        UtcTimestamp LastUpdatedDate { get; set; }
        EntityHeader CreatedBy { get; set; }
        EntityHeader LastUpdatedBy { get; set; }
        EntityHeader ClonedFromId { get; set; }
        EntityHeader ClonedFromOrg { get; set; }
        EntityHeader ClonedRevision { get; set; }

        bool IsDraft { get; }
    }

    public interface IAuditableEntity : IAuditableEntitySimple
    {
        List<EntityChangeSet> AuditHistory { get; set; }

        bool IsDeprecated { get; set; }
        EntityHeader DeprecatedBy { get; set; }
        UtcTimestamp? DeprecationDate { get; set; }
        String DeprecationNotes { get; set; }

        string ETag { get; set; }
    }
}
