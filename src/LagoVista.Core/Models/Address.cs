using LagoVista.Core.Attributes;
using LagoVista.Core.Models.Geo;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public enum AddressTypes
    {
        [EnumLabel(Address.AddressType_Business, Resources.LagoVistaCommonStrings.Names.Address_AddressType_Business, typeof(Resources.LagoVistaCommonStrings))]
        Business,
        [EnumLabel(Address.AddressType_Residential, Resources.LagoVistaCommonStrings.Names.Address_AddressType_Residential, typeof(Resources.LagoVistaCommonStrings))]
        Residential,
        [EnumLabel(Address.AddressType_Other, Resources.LagoVistaCommonStrings.Names.Address_AddressType_Other, typeof(Resources.LagoVistaCommonStrings))]
        Other
    }

    [EntityDescription(LGVCommonDomain.CommonDomain, Resources.LagoVistaCommonStrings.Names.Address_Title, Resources.LagoVistaCommonStrings.Names.Address_Help,
        Resources.LagoVistaCommonStrings.Names.Address_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(Resources.LagoVistaCommonStrings))]
    public class Address
    {
        public const string AddressType_Business = "business";
        public const string AddressType_Residential = "residential";
        public const string AddressType_Other = "other";

        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_AddressType, FieldType: FieldTypes.Picker, WaterMark: Resources.LagoVistaCommonStrings.Names.Address_AddressType_Select, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string AddressType { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_GeoLocation, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public GeoLocation GeoLocation { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_Address1, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string Address1 { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_Address2, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string Address2 { get; set; }
        public string UnitNumber { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_City, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string City { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_StateOrProvince, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string StateOrProvince { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_PostalCode, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string PostalCode { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Address_Country, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string Country { get; set; }
        [FormField(LabelResource: Resources.LagoVistaCommonStrings.Names.Common_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(Resources.LagoVistaCommonStrings))]
        public string Notes { get; set; }
    }
}
