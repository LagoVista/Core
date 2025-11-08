// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0b33250c34037aabc9497fc7d223a8c66a9c45109a83cee31592fe068fc0fd96
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface IUserInfo
    {
        String Id { get; set; }

        String CreationDate { get; set; }

        EntityHeader CreatedBy { get; set; }

        String LastUpdatedDate { get; set; }

        EntityHeader LastUpdatedBy { get; set; }

        bool EmailConfirmed { get; set; }

        string FirstName { get; set; }
        string LastName { get; set; }

        bool IsSystemAdmin { get; set; }

        bool IsOrgAdmin { get; set; }

        string PhoneNumber { get; set; }
        bool PhoneNumberConfirmed { get; set; }

        EntityHeader CurrentOrganization { get; set; }

        ImageDetails ProfileImageUrl { get; set; }

        string Email { get; set; }

        List<EntityHeader> Roles { get; set; }
    }
}
