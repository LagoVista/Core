// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e4b5345b334d8680ed116b0c76b29eaa80bb2484731f7bea69c54a68fb61f1f9
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LagoVista.Models
{
    [Table("Org", Schema = "dbo")]
    public class OrganizationDTO : IEntityHeaderFactory
    {

        // Don't be tempted to convert this to NormalizeId32, this is the database side that has to be a string.
        [MapTo("Id")]
        [Key]
        public string OrgId { get; set; }
        [MapTo("Text")]
        [Required]
        public string OrgName { get; set; }
        [Required]
        public string OrgBillingContactId { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastUpdatedDate { get; set; }

        [IgnoreOnMapTo]
        public AppUserDTO BillingContact { get; set; }

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(OrgId) || String.IsNullOrEmpty(OrgName);
        }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(OrgId, OrgName);
        }
    }
}
