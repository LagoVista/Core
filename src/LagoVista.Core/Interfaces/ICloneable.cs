// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 78845da7d3b67fc22fe8b40fe9bcb25392685e8cc215f577663f1761ff3492d3
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ICloneable 
    {        
    }

    public interface ICloneable<TEntity> : ICloneable where TEntity : IIDEntity, IKeyedEntity, INamedEntity, IOwnedEntity, IAuditableEntity
    {
        Task<TEntity> CloneAsync(EntityHeader user, EntityHeader org, string name = "", string key = "");

        string OriginalId { get; set; }

        EntityHeader OriginalOwnerOrganization { get; set; }
        EntityHeader OriginalOwnerUser { get; set; }

        EntityHeader OriginallyCreatedBy { get; set; }

        string OriginalCreationDate { get; set; }
    }
}
