using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LagoVista.Core;
using System.Threading.Tasks;
using LagoVista.Core.Cloning;
using LagoVista.Core.Attributes;

namespace LagoVista.Core.Tests.CloneableTests
{
    public class ParentModel : ICloneable<ParentModel>, IOwnedEntity, IIDEntity, IKeyedEntity, INamedEntity, IAuditableEntity
    {
        public ParentModel()
        {
            Id = Guid.NewGuid().ToId();
        }

        [CloneOptions(false)]
        public string Id { get; set; }

        [CloneOptions(false)]
        public bool IsPublic { get; set; }

        [CloneOptions(false)]
        public EntityHeader OwnerOrganization { get; set; }

        [CloneOptions(false)]
        public EntityHeader OwnerUser { get; set; }

        [CloneOptions(false)]
        public string Key { get; set; }

        [CloneOptions(false)]
        public string Name { get; set; }

        [CloneOptions(false)]
        public string CreationDate { get; set; }

        [CloneOptions(false)]
        public string LastUpdatedDate { get; set; }

        [CloneOptions(false)]
        public EntityHeader CreatedBy { get; set; }

        [CloneOptions(false)]
        public EntityHeader LastUpdatedBy { get; set; }

        [CloneOptions(true)]
        public ChildModel1 Child1 {get; set;}

        [CloneOptions(true)]
        public List<ChildModel1> Children1 { get; set; }

        [CloneOptions(true)]
        public ChildModel2 Child2 { get; set; }

        [CloneOptions(true)]
        public List<ChildModel2> Children2 { get; set; }

        [CloneOptions(false)]
        public string OriginalId { get; set; }

        [CloneOptions(false)]
        public EntityHeader OriginalOwnerOrganization { get; set; }

        [CloneOptions(false)]
        public EntityHeader OriginalOwnerUser { get; set; }

        [CloneOptions(false)]
        public EntityHeader OriginallyCreatedBy { get; set; }

        [CloneOptions(false)]
        public string OriginalCreationDate { get; set; }


        public Task<ParentModel> CloneAsync(EntityHeader user, EntityHeader org, string name, string key)
        {
            return Task.FromResult(CloneService.Clone(this, user, org, name, key));
        }
    }
}
