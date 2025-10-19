// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 261899d1e4d6726bfe5f2aeb8caa5bd9641babd1fbbe7d13307dd8ce84d8797f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;

namespace LagoVista.Core.Interfaces
{
    public interface ISoftDeletable
    {
        bool? IsDeleted { get; set; }
        EntityHeader DeletedBy { get; set; }
        string DeletionDate { get; set; }
    }
}
