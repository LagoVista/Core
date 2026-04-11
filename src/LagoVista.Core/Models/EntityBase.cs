// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 83d87a7e34b4e0c2b0c21cedd51bf92b2d7182f4556928c09e9b99b232c5d5e7
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.AI.Interfaces;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using LagoVista.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.Models
{
    public interface IEntityBase : INoSQLEntity, IOwnedEntity, IIconEntity, IKeyedEntity, IIDEntity, INamedEntity, IAuditableEntity, IEntityHeaderEntity, IRevisionedEntity, ISoftDeletable, ICategorized, IRatedEntity, ILabeledEntity, IDescriptionEntity, IAISessionTracker
    {

    }

    public class EntityBase : ModelBase, IEntityBase
    {
        // Persisted Cosmos id field
        [AiSchemaIgnore]
        [JsonProperty("id")]
        [EditorBrowsable(EditorBrowsableState.Never)] 
        public string StoredId { get; set; }

        [AiSchemaIgnore]
        // You already have this
        public string EntityType { get; set; }

        // Canonical identity for the app/domain
        [AiSchemaIgnore]
        [JsonIgnore] // optional; without it, Json.NET still won't serialize it unless asked, but I like being explicit
        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Id, FieldType: FieldTypes.ReadonlyLabel, AiChatPrompt: "Do not show to the user unless they explicity ask to view it.", ResourceType: typeof(LagoVistaCommonStrings))]
        public NormalizedId32 Id
        {
            get => GetCanonicalIdOrThrow();
            set => StoredId = value.Value; // new writes store normalized
        }

        private NormalizedId32 GetCanonicalIdOrThrow()
        {
            if (StoredId == null) throw new ArgumentNullException(nameof(StoredId));

            if (NormalizedId32.IsNormalizedId32(StoredId))
                return new NormalizedId32(StoredId);

            // Surgical legacy Product exception
            if (AllowsLegacyGuidId(this.GetType()) && GuidString36.IsStrictLowerD(StoredId))
            {
                return new GuidString36(StoredId).ToNormalizedId32();
            }

            throw new FormatException(
                $"Invalid Cosmos id '{StoredId}' for EntityType '{EntityType}'. Expected NormalizedId32" +
                (string.Equals(EntityType, "ProductEntity", StringComparison.Ordinal) ? " or legacy GuidString36." : "."));
        }

        private static bool AllowsLegacyGuidId(Type t) => Attribute.IsDefined(t, typeof(AllowLegacyGuidDocumentIdAttribute), inherit: true);

        public EntityBase()
        {
            Id = Guid.NewGuid().ToId();
            Revision = 1;
            RevisionTimeStamp = DateTime.UtcNow.ToJSONString();
        }

        [AiSchemaIgnore]
        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_IsPublic, ResourceType: typeof(LagoVistaCommonStrings), FieldType:FieldTypes.CheckBox, IsUserEditable: true)]
        public bool IsPublic { get; set; }
        [AiSchemaIgnore]
        public EntityHeader PublicPromotedBy { get; set; }
        [AiSchemaIgnore]
        public string PublicPromotionDate { get; set; }

        [AiSchemaIgnore]
        [CloneOptions(false)]
        public EntityHeader OwnerOrganization { get; set; }

        [AiSchemaIgnore]
        [CloneOptions(false)]
        public EntityHeader OwnerUser { get; set; }

        [AiSchemaIgnore]
        public string DatabaseName { get; set; }

        [AiSchemaIgnore]
        [CloneOptions(false)]
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        private string _name;
        [AiSchemaIgnore]
        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public virtual string Name
        {
            get => _name;
            set => Set(ref _name, value);   
        }

        [AiSchemaIgnore]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true)]
        public LagoVistaIcon Icon { get; set; } = "none";

        [AiSchemaIgnore]
        [CloneOptions(false)]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Key, HelpResource: LagoVistaCommonStrings.Names.Common_Key_Help, FieldType: FieldTypes.Key, AiChatPrompt:"When isDraft is not TRUE, the key MUST NEVER be updated.", 
            RegExValidationMessageResource: LagoVistaCommonStrings.Names.Common_Key_Validation, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true)]
        public virtual LagoVistaKey Key { get; set; }

        [AiSchemaIgnore]
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: LagoVistaCommonStrings.Names.Common_Category_Select, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }


        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Description, IsRequired: false, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Description { get; set; }

        [AiSchemaIgnore]
        public List<EntityHeader> AISessions { get; set; } = new List<EntityHeader>();

        [AiSchemaIgnore]
        [CloneOptions(false)]
        public List<EntityChangeSet> AuditHistory { get; set; } = new List<EntityChangeSet>();
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public bool? IsDeleted { get; set; } = false;
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public EntityHeader DeletedBy { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public UtcTimestamp? DeletionDate { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public bool IsDeprecated { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public EntityHeader DeprecatedBy { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public UtcTimestamp? DeprecationDate { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public string DeprecationNotes { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public UtcTimestamp CreationDate { get; set; }

        [AiSchemaIgnore]
        [CloneOptions(false)]
        public UtcTimestamp LastUpdatedDate { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public EntityHeader CreatedBy { get; set; }
        [CloneOptions(false)]
        [AiSchemaIgnore]
        public EntityHeader LastUpdatedBy { get; set; }

        [AiSchemaIgnore]
        public EntityHeader ClonedFromId { get; set; }
        [AiSchemaIgnore]
        public EntityHeader ClonedFromOrg { get; set; }
        [AiSchemaIgnore]
        public EntityHeader ClonedRevision { get; set; }

        [AiSchemaIgnore]
        public string Sha256Hex {get; set;}

        [AiSchemaIgnore]
        public bool IsDraft { get; set; } = false;

        [AiSchemaIgnore]
        public int Revision { get; set; }
        [AiSchemaIgnore]
        public string RevisionTimeStamp { get; set; }

        [AiSchemaIgnore]
        public List<Label> Labels { get; set; } = new List<Label>();


        [AiSchemaIgnore]
        public double? Stars { get; set; }
        [AiSchemaIgnore]
        public int RatingsCount { get; set; }

        [AiSchemaIgnore]
        public List<EntityRating> Ratings { get; set; } = new List<EntityRating>();


        public EntityHeader ToEntityHeader()
        {
            var entityHheader = new EntityHeader()
            {
                Id = this.Id,
                Key = this.Key,
                Text = this.Name,
                EntityType = this.EntityType,
                OwnerOrgId = this.OwnerOrganization?.Id
            };
            return entityHheader;
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

        public void AddChange(string fieldName, string oldValue, string newValue, EntityHeader user = null, string timeStamp = null)
        {
            AuditHistory.Add(new EntityChangeSet()
            {
                ChangeDate = timeStamp ?? LastUpdatedDate,
                ChangedBy = user ?? LastUpdatedBy,
                Changes = new List<EntityChange>()
                {
                   new EntityChange()
                   {
                        Field = fieldName,
                        OldValue = oldValue,
                        NewValue = newValue,
                   }
                }
            });
        }
    }
}
