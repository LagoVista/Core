// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f02b5c80baa228e85e8cb04d09b5daf8b5763248d5eaf6a58b7d2edafed60c40
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IOwnedEntity:IIDEntity
    {
        bool IsPublic { get; set; }
        EntityHeader OwnerOrganization { get; set; }
        EntityHeader OwnerUser { get; set; }
    
        EntityHeader PublicPromotedBy { get; set; }
        string PublicPromotionDate { get; set; }
    }
}
