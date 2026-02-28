using LagoVista.Core.Models;
using LagoVista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Repos
{
    public interface IAppUserRelationalRepo
    {
        Task AddAppUserAsync(AppUserDTO user, EntityHeader org, EntityHeader createdByUser);
        Task<AppUserDTO> GetAppUserAsync(string id, EntityHeader org, EntityHeader user);
        Task UpdateAppUserAsync(AppUserDTO user, EntityHeader org, EntityHeader updatedByUser);
        Task DeleteAppUserAsync(string id, EntityHeader org, EntityHeader user);
    }
}
