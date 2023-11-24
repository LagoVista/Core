using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Cloning;
using LagoVista.Core.Attributes;

namespace LagoVista.Core.Tests.CloneableTests
{
    public class ParentModel : EntityBase, ICloneable<ParentModel>
    {
        public ParentModel()
        {
            Id = Guid.NewGuid().ToId();
        }

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


        public List<EntityChangeSet> History { get; set; } = new List<EntityChangeSet>();
        public Task<ParentModel> CloneAsync(EntityHeader user, EntityHeader org, string name, string key)
        {
            return Task.FromResult(CloneService.Clone(this, user, org, name, key));
        }
    }
}
