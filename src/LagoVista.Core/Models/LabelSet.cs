using LagoVista.Core.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    public class LabelSet : INoSQLEntity, IIDEntity, IOwnedEntity, IAuditableEntity
    {
        public LabelSet()
        {
            Labels = new List<Label>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }
        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }
        public List<Label> Labels { get; set; }
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }
    }

    public class Label
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
    }
}
