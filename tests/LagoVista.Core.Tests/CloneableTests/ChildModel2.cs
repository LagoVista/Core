using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.CloneableTests
{
    public class ChildModel2 : ICloneable<ChildModel2>, IOwnedEntity, IIDEntity, IKeyedEntity, INamedEntity, IAuditableEntity
    {
        public string Id { get; set; }
        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }        
        public string Key { get; set; }
        public string Name { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }

        public ChildModel2 GrandChild { get; set; }
        public string OriginalId { get; set; }
        public EntityHeader OriginalOwnerOrganization { get; set; }
        public EntityHeader OriginalOwnerUser { get; set; }
        public EntityHeader OriginallyCreatedBy { get; set; }
        public string OriginalCreationDate { get; set; }

        public Task<ChildModel2> CloneAsync(EntityHeader user, EntityHeader org, string name, string key)
        {
            return this.CloneAsync(user, org, name, key);
        }
    }
}
