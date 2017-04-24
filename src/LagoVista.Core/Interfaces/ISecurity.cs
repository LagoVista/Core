using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.Core.Interfaces
{
    public interface ISecurity
    {
        Task AuthorizeAsync(IOwnedEntity ownedEntity, AuthorizeActions action, EntityHeader user, EntityHeader org = null);
        Task AuthorizeOrgAccess(EntityHeader user, EntityHeader org, Type entityType = null);
        Task AuthorizeOrgAccess(EntityHeader user, string orgId, Type entityType = null);
        Task AuthorizeOrgAccess(string userId, string orgId, Type entityType = null);
    }
}
