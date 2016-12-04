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
