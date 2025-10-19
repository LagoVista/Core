// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 224abb37c83ee99d665ccca99f35b30e04e5c216c4cc1570abd5b528ed8aee9f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.CloneableTests
{
    public class ChildModel2 : EntityBase, ICloneable<ChildModel2>, IOwnedEntity
    {
      
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
