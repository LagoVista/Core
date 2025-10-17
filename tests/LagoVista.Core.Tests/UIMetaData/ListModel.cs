// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cf462c7b1d6e5efa281be54e7cbac109d5a2e0df3934ad077459ea3b3068eb12
// IndexVersion: 1
// --- END CODE INDEX META ---
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
