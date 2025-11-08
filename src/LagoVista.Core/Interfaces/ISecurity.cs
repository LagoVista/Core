// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 97123dadc968cf387e27c3190fc12eb7d607cb852a95570f345c4d48d5557121
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.Core.Interfaces
{
    public interface ISecurity
    {
        Task AuthorizeAsync(IOwnedEntity ownedEntity, AuthorizeActions action, EntityHeader user, EntityHeader org, string actionName);
        Task AuthorizeAsync(EntityHeader user, EntityHeader org, string actionName, Object data );
        Task AuthorizeFinanceAdminAsync(EntityHeader user, EntityHeader org, string actionName, Object data);
        Task AuthorizeAsync(EntityHeader user, EntityHeader org, Type entityType, Actions action, string id);
        Task AuthorizeAsync(EntityHeader user, EntityHeader org, Type entityType, Actions action);
        Task AuthorizeAsync(string user, string org, string actionName, Object data );
        Task AuthorizeOrgAccessAsync(EntityHeader user, EntityHeader org, Type entityType = null, Actions action = Actions.Any);
        Task AuthorizeOrgAccessAsync(EntityHeader user, string orgId, Type entityType = null, Actions action = Actions.Any);
        Task AuthorizeOrgAccessAsync(string userId, string orgId, Type entityType = null, Actions action = Actions.Any);
        Task LogEntityActionAsync(String id, string entityType, string accessType, EntityHeader org, EntityHeader user);

        Task<List<EntityHeader>> GetUserRolesAsync(string userId, string orgId);
    
        Task<bool> DoesUserHaveRoleAsync(string userId, string orgId, string roleName);
    }
}
