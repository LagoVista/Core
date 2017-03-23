using LagoVista.Core.Attributes;
using LagoVista.Core.Tests.Resources.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.UIMetaData
{
    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Model3_Title, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class ListModel
    {
        [ListColumn(MetaDataResources.Names.Field3_Column_Header, ResourceType: (typeof(MetaDataResources)))]
        public String Prop1 { get; set; }
        [ListColumn(MetaDataResources.Names.Field3_Column_Header, ResourceType:(typeof(MetaDataResources)))]
        public String Prop2 { get; set; }

    }
}
