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
        Task AddAppUserAsync(AppUserDTO user);
        Task<AppUserDTO> GetAppUserAsync(string id);
        Task UpdateAppUserAsync(AppUserDTO user);
        Task DeleteAppUserAsync(string id);
    }
}
