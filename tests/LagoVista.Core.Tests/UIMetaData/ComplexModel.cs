// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 61966092bab98b20b9227745dbb2ffb1e008d333d45e2c4df02a0bc0ff035c21
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Tests.Resources.UIMetaData;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.UIMetaData
{
    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Complex_Model, MetaDataResources.Names.Complex_Model_Help, MetaDataResources.Names.Complex_Model_Help, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class ComplexModel
    {
        public ComplexModel()
        {
            ChildList1 = new List<Child1>();
            ChildList2 = new List<Child2>(); 
        }

        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark,
            HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }

        [FormField(LabelResource: MetaDataResources.Names.ChildList1, FieldType: FieldTypes.ChildList, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public List<Child1> ChildList1 { get; set; }
      
        [FormField(LabelResource: MetaDataResources.Names.ChildList2, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public List<Child2> ChildList2 { get; set; }

    }

    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Child1, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class Child1
    {
        public Child1()
        {
            SubChildList1 = new List<SubChild1>();
            SubChildList2 = new List<SubChild2>();
        }

        [FormField(LabelResource: MetaDataResources.Names.SubChildList1, FieldType: FieldTypes.ChildList, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public List<SubChild1> SubChildList1 { get; set; }
       
        [FormField(LabelResource: MetaDataResources.Names.SubChildList2, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public List<SubChild2> SubChildList2 { get; set; }

        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark,
            HelpResource: MetaDataResources.Names.Field1_Help, FieldType: FieldTypes.Text,  IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }
    }

    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Child2, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class Child2
    {
        public Child2()
        {
            SubChildList3 = new List<SubChild3>();
        }

        [FormField(LabelResource: MetaDataResources.Names.SubChildList3, FieldType:FieldTypes.ChildList, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public List<SubChild3> SubChildList3 { get; set; }

        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark,
            HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }
    }

    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Model3_Title, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class SubChild1
    {
        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark, 
            HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }
    }

    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Model3_Title, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class SubChild2
    {
        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark,
            HelpResource: MetaDataResources.Names.Field1_Help, FieldType: FieldTypes.Text, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }

    }

    [EntityDescription(Domains.MetaData1, MetaDataResources.Names.Model3_Title, MetaDataResources.Names.Model3_Help, MetaDataResources.Names.Model3_Description, EntityDescriptionAttribute.EntityTypes.BusinessObject, typeof(MetaDataResources))]
    public class SubChild3
    {
        [FormField(LabelResource: MetaDataResources.Names.Field1_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.Field1_WaterMark,
            HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.Field1_RequiredMessage1)]
        public String Field1 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field2_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field2 { get; set; }
        [FormField(LabelResource: MetaDataResources.Names.Field3_Label, FieldType: FieldTypes.Text, ResourceType: typeof(MetaDataResources), HelpResource: MetaDataResources.Names.Field1_Help, IsRequired: true)]
        public String Field3 { get; set; }
    }
}
