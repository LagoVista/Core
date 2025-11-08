// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1302381b25b872b2ef43d3a0103d0860440ebb48b4d4fbae905be8c1db7ac796
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Tests.Resources.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.UIMetaData
{
    [EntityDescription(Domains.MetaData2, MetaDataResources.Names.Model3_Title, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(MetaDataResources) )]
    public class Model3
    {
        [FormField(ColHeaderResource:MetaDataResources.Names.Field1_Summary, ResourceType: typeof(MetaDataResources))]
        public String Field1 { get; set; }
        [FormField(ColHeaderResource: MetaDataResources.Names.Field2_Summary, ResourceType: typeof(MetaDataResources))]
        public String Field2 { get; set; }
        [FormField(ColHeaderResource: MetaDataResources.Names.Field1_Summary, ResourceType: typeof(MetaDataResources))]
        public String Field3 { get; set; }

    }
}
