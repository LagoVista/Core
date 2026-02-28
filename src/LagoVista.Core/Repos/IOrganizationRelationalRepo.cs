using LagoVista.Core.Models;
using LagoVista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Repos
{
    public interface IOrganizationRelationalRepo
    {
        Task AddOrganizationAsync(OrganizationDTO organization, EntityHeader org, EntityHeader user);
        Task<OrganizationDTO> GetOrganizationAsync(string id, EntityHeader org, EntityHeader user);
        Task UpdateOrganizationAsync(OrganizationDTO organization, EntityHeader org, EntityHeader user);
        Task DeleteOrganizationAsync(string id, EntityHeader org, EntityHeader user);
    }
}
