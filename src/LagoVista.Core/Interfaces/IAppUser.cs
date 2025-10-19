// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 28ef10dca252984d44c7515bc5d503a1a7abdd3ec565280a368ab543309d735f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IAppUser : IIDEntity, IAuditableEntity
    {
        string UserName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string EndUserOrgAppId { get; set; }
        string Email { get; set; }
        bool EmailConfirmed { get; set; }        
        string PhoneNumber { get; set; }
        bool PhoneNumberConfirmed { get; set; }
        ImageDetails ProfileImageUrl { get; set; }
        IOrganizationSummary CurrentOrganization { get; set; }
        List<ExternalLogin> ExternalLogins { get; set; }
        List<EntityHeader> Organizations { get; set; }
        List<EntityHeader> CurrentOrganizationRoles { get; set; }
        Dictionary<string, string> Preferences { get; set; } 
    }
}