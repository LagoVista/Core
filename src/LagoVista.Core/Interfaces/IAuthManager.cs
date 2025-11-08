// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d1f0cbe91f249ee23c6e85db5fc086460bced498916941ed1f77cbf9ef6309dc
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IAuthManager
    {
        event EventHandler<EntityHeader> OrgChanged;

        event EventHandler<List<EntityHeader>> RolesChanged;

        void RaiseOrgChanged(EntityHeader newOrg);

        void RaiseRolesChanged(List<EntityHeader> newRoles);


        string AccessToken { get; set; }
        string AccessTokenExpirationUTC { get; set; }
        string AppInstanceId { get; set; }
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        bool IsAuthenticated { get; set; }
        string RefreshToken { get; set; }
        string RefreshTokenExpirationUTC { get; set; }
        UserInfo User { get; set; }
        Task LoadAsync();
        Task PersistAsync();
        Task LogoutAsync();

        List<EntityHeader> Roles { get; set; }
    }
}
