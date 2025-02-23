using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityBase: ModelBase, INoSQLEntity, IOwnedEntity, IKeyedEntity, IIDEntity, INamedEntity, IAuditableEntity, IEntityHeaderEntity, IRevisionedEntity, ISoftDeletable
    {

        public EntityBase()
        {
            Id = Guid.NewGuid().ToId();
            Revision = 1;
            RevisionTimeStamp = DateTime.UtcNow.ToJSONString();
        }

        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_IsPublic, ResourceType: typeof(LagoVistaCommonStrings), FieldType:FieldTypes.CheckBox, IsUserEditable: true)]
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


        private string _name;
        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public virtual string Name
        {
            get => _name;
            set => Set(ref _name, value);   
        }       

        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Key, HelpResource: LagoVistaCommonStrings.Names.Common_Key_Help, FieldType: FieldTypes.Key, 
            RegExValidationMessageResource: LagoVistaCommonStrings.Names.Common_Key_Validation, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true)]
        public virtual string Key { get; set; }

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

        public EntityHeader ClonedFromId { get; set; }
        public EntityHeader ClonedFromOrg { get; set; }
        public EntityHeader ClonedRevision { get; set; }

        public int Revision { get; set; }
        public string RevisionTimeStamp { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(Id, Key, Name);
        }

        private Dictionary<string, object> _originalValues = new Dictionary<string, object>();

        public void SetOriginalValues()
        {
            var properties = this.GetType().GetRuntimeProperties();
            foreach(var property in properties) 
            {
                _originalValues.Add(property.Name, property.GetValue(this));
            }
        }

        public Dictionary<string, object>  GetChanges()
        {
            var properties = this.GetType().GetRuntimeProperties();
            var latestChanges = new Dictionary<string, object>();

            // Save the current value of the properties to our dictionary.
            foreach (PropertyInfo property in properties)
            {
                latestChanges.Add(property.Name, property.GetValue(this));
            }

            PropertyInfo[] tempProperties = this.GetType().GetRuntimeProperties().ToArray();

            // Filter properties by only getting what has changed
            properties = tempProperties.Where(p => !Equals(p.GetValue(this, null), this._originalValues[p.Name])).ToArray();

            foreach (PropertyInfo property in properties)
            {
                latestChanges.Add(property.Name, property.GetValue(this));
            }

            return latestChanges;
        }
    }
}
