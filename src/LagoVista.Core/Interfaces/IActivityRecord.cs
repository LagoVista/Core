using System;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Minimal record contract for immutable activity records.
    /// Storage providers may map each concrete record type to its own physical table or collection.
    /// </summary>
    public interface IActivityRecord
    {
        string Id { get; set; }
        string OrganizationId { get; set; }
        string Organization { get; set; }
        DateTime CreationDate { get; set; }
    }
}
