using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Text;

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

        [ListColumn(HeaderResource: Resources.LagoVistaCommonStrings.Names.ListResponse_Category, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Category { get; set; }
        public string CategoryId { get; set; }
        public string CategoryKey { get; set; }

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
         }
    }

}
