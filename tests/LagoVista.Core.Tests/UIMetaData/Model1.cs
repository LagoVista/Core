using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.UIMetaData
{
    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Model3_Title, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class Model1 : BaseModel1
    {
        public enum EnumValues
        {
            [EnumLabel("ENUM1", MetaDataResources.Names.Enum1_Label, typeof(MetaDataResources), MetaDataResources.Names.Enum1_Help)]
            EnumValue1,
            [EnumLabel("ENUM2", MetaDataResources.Names.Enum2_Label, typeof(MetaDataResources), MetaDataResources.Names.Enum2_Help)]
            EnumValue2,
            [EnumLabel("ENUM3", MetaDataResources.Names.Enum3_Label, typeof(MetaDataResources), MetaDataResources.Names.Enum3_Help)]
            EnumValue3
        }


        [FormField(LabelResource:MetaDataResources.Names.Field1_Label, ResourceType:typeof(MetaDataResources), WaterMark:MetaDataResources.Names.Field1_WaterMark, HelpResource:MetaDataResources.Names.Field1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, ResourceType: typeof(MetaDataResources), HelpResource:MetaDataResources.Names.Field2_Help, RegExValidationMessageResource:MetaDataResources.Names.Field2_RegExMessage)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, ResourceType: typeof(MetaDataResources))]
        public String Field3 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field4_Label, ResourceType: typeof(MetaDataResources))]
        public String Field4 { get; set; }


        [FormField(LabelResource: MetaDataResources.Names.Field4_Label, FieldType:FieldTypes.Picker, EnumType:typeof(EnumValues), ResourceType: typeof(MetaDataResources))]
        public EntityHeader EnumField { get; set; }

        [FormField(LabelResource: MetaDataResources.Names.Field5_Label, FieldType: FieldTypes.ChildView, ResourceType: typeof(MetaDataResources))]
        public Model2 Field5 { get; set; }
    }
}
