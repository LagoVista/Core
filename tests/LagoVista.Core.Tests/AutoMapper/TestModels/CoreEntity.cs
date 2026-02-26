using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Models;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class CoreEntity
    {
        public string Id { get; set; }
     
        [MapTo(nameof(DbModelBase.CreatedById))]
        public EntityHeader CreatedBy { get; set; }
        
        [MapTo(nameof(DbModelBase.LastUpdatedById))]
        public EntityHeader LastUpdatedBy { get; set; }

        [MapTo(nameof(DbModelBase.OrganizationId))]
        public EntityHeader OwnerOrganization { get; set; }
        
        public string CreationDate { get; set; }

        [MapTo(nameof(DbModelBase.LastUpdateDate))]
        public string LastUpdatedDate { get; set; }
    }

    public class EntityHeaderPrimary
    {

        [MapToNameAndId("Id", "Text")]
        public EntityHeader TheProperty { get; set; }

    }

    public class EntityHeaderDTO
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
