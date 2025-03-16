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
