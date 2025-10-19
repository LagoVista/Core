// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bcf145454b01caa6b7f0feab4c225a10be969dccd40e6e511168ce798a21b806
// IndexVersion: 0
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
    public class Model2
    {

        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark, HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, ResourceType: typeof(MetaDataResources),  HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, ResourceType: typeof(MetaDataResources),  HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }
    }
}
