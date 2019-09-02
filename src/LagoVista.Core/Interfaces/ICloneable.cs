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
