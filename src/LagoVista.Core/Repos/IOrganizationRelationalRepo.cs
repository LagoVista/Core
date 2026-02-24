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
        Task AddOrganizationAsync(OrganizationDTO organization);
        Task<OrganizationDTO> GetOrganizationAsync(string id);
        Task UpdateOrganizationAsync(OrganizationDTO organization);
        Task DeleteOrganizationAsync(string id);
    }
}
