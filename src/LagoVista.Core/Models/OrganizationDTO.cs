// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e4b5345b334d8680ed116b0c76b29eaa80bb2484731f7bea69c54a68fb61f1f9
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LagoVista.Models
{
    public class OrganizationDTO 
    {
        [Key]
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgBillingContactId { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(OrgId, OrgName);
        }
    }
}
