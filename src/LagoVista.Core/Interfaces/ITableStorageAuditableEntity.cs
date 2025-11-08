// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0ab4763327db529dbfb99a49e299c4617dc338a0ba8ed5e93e0d74500dbabab5
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Interfaces
{
    public interface ITableStorageAuditableEntity
    {
        String CreatedBy { get; set; }
        String CreatedById { get; set; }
        String CreationDate { get; set; }
        String LastUpdatedBy { get; set; }
        String LastUpdatedById { get; set; }
        String LastUpdatedDate { get; set; }
    }
}
