// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 50806d0984019deb7270a36c4b36fe7ad98a25c552496aecc0595a3c21ae210e
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using System;
using System.Linq;

namespace LagoVista.Core.Models
{
    public class SummaryData : ISummaryData
    {
        [ListColumn(Visible: false)]
        public String Id { get; set; }

        public string Icon { get; set; } = "icon-ae-document";
   
        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.Common_IsPublic, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool IsPublic { get; set; }

        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.Common_Name, ResourceType: typeof(LagoVistaCommonStrings))]
        public String Name { get; set; }

        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.Common_Key, ResourceType: typeof(LagoVistaCommonStrings))]
        public String Key { get; set; }

        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.Common_Description, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Description { get; set; }

        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.ListResponse_Deleted, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool? IsDeleted { get; set; }

        public bool IsDraft { get; set; }

        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.ListResponse_Category, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Category { get; set; }
        public string CategoryId { get; set; }
        public string CategoryKey { get; set; }

        public int? DiscussionsTotal { get; set; }
        public int? DiscussionsOpen { get; set; }

        public double? Stars { get; set; }
        public int RatingsCount { get; set; }

        public string LastUpdatedDate { get; set; }

        public void Populate(EntityBase entity)
        {
            Id = entity.Id;
            Category = entity.Category?.Text;
            CategoryId = entity.Category?.Id;
            CategoryKey = entity.Category?.Key;
            IsPublic = entity.IsPublic;
            IsDeleted = entity.IsDeleted;
            Name = entity.Name;
            Key = entity.Key;
            LastUpdatedDate = entity.LastUpdatedDate;
            Stars = entity.Stars;
            IsDraft = entity.IsDraft;
            RatingsCount = entity.RatingsCount;
            var discussableEntity = entity as IDiscussableEntity;
            if (discussableEntity != null)
            {
                DiscussionsTotal = discussableEntity.Discussions.Count;
                DiscussionsOpen = discussableEntity.Discussions.Where(dsc=>dsc.Open).Count();
            }
        }
    }

}
