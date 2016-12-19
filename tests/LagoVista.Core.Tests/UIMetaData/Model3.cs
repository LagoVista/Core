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
