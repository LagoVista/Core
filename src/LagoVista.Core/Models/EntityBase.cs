using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityBase: ModelBase, INoSQLEntity, IOwnedEntity, IKeyedEntity, IIDEntity, INamedEntity, IAuditableEntity, IEntityHeaderEntity
    {

        public EntityBase()
        {
            Id = Guid.NewGuid().ToId();
        }

        [CloneOptions(false)]
        public bool IsPublic { get; set; }

        [CloneOptions(false)]
        public EntityHeader OwnerOrganization { get; set; }

        [CloneOptions(false)]
        public EntityHeader OwnerUser { get; set; }

        [CloneOptions(false)]
        [JsonProperty("id")]
        public string Id { get; set; }
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Name, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public string Name { get; set; }

        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Key, HelpResource: LagoVistaCommonStrings.Names.Common_Key_Help, FieldType: FieldTypes.Key, 
            RegExValidationMessageResource: LagoVistaCommonStrings.Names.Common_Key_Validation, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true)]
        public string Key { get; set; }

        [CloneOptions(false)]
        public List<EntityChangeSet> AuditHistory { get; set; } = new List<EntityChangeSet>();
        [CloneOptions(false)]
        public bool IsDeleted { get; set; }
        [CloneOptions(false)]
        public EntityHeader DeletedBy { get; set; }
        [CloneOptions(false)]
        public string DeletionDate { get; set; }
        [CloneOptions(false)]
        public bool IsDeprecated { get; set; }
        [CloneOptions(false)]
        public EntityHeader DeprecatedBy { get; set; }
        [CloneOptions(false)]
        public string DeprecationDate { get; set; }
        [CloneOptions(false)]
        public string DeprecationNotes { get; set; }
        [CloneOptions(false)]
        public string CreationDate { get; set; }
        [CloneOptions(false)]
        public string LastUpdatedDate { get; set; }
        [CloneOptions(false)]
        public EntityHeader CreatedBy { get; set; }
        [CloneOptions(false)]
        public EntityHeader LastUpdatedBy { get; set; }

        public IEntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(Id, Key, Name);
        }
    }
}
