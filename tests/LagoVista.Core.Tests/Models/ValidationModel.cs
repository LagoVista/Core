// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3ac6cedb9d11bf8ea5c7a0f34596cf964fd3a5ddab26f39b2e7adee72dfd26e9
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using LagoVista.Core.Tests.Resources.UIMetaData;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Models
{
    public class ValidationModelBase
    {
        [FormField(IsRequired: true)]
        public String ParentRequiredProperty { get; set; }

    }

    public class ValidationModel : ValidationModelBase, IValidateable
    {
        [FormField(IsRequired: true)]
        public String PropertyBasedValidationMessage { get; set; }


        [FormField(IsRequired:true, LabelResource: ValidationResources.Names.FirstNameLabel, ResourceType: typeof(ValidationResources))]
        public string LabelBasedValidationMessage { get; set; }


        [FormField(IsRequired: true, ReqMessageResource: ValidationResources.Names.CustomValidationMessage, ResourceType: typeof(ValidationResources))]
        public String CustomValidationMessage { get; set; }

        [FormField(IsRequired:true)]
        public String SystemRequired { get; set; }
    }

    public class IdValidationModel : IIDEntity,IValidateable
    {
        public string Id
        {
            get; set;
        }
    }

    public class AuditableModel : EntityBase, IValidateable
    {

    }

    public enum EnumFortTesting
    {
        [EnumLabel("value1", LabelResource: MetaDataResources.Names.EntityHeader_Enum_Value1, ResourceType: typeof(ValidationResources))]
        Value1,
        [EnumLabel("value2", LabelResource: MetaDataResources.Names.EntityHeader_Enum_Value2, ResourceType: typeof(ValidationResources))]
        Value2,
        [EnumLabel("value3", LabelResource: MetaDataResources.Names.EntityHeader_Enum_Value3, ResourceType: typeof(ValidationResources))]
        Value3
    }

    public class EntityHeaderModel : IValidateable
    {
        [FormField(IsRequired:true)]
        public EntityHeader SystemRequired { get; set; }

        [FormField(IsRequired: true, LabelResource: ValidationResources.Names.FirstNameLabel, ResourceType: typeof(ValidationResources))]
        public EntityHeader UserRequired { get; set; }

        [FormField(IsRequired: true, ReqMessageResource: ValidationResources.Names.CustomValidationMessage, ResourceType: typeof(ValidationResources))]
        public EntityHeader UseRequiredCustomMessage { get; set; }

        [FormField()]
        public EntityHeader NotRequired { get; set; }

        [FormField(IsRequired: true)]
        public EntityHeader<EnumFortTesting> PropWithEnum { get; set; }
    }

    public class EntityHeaderChildValueModels : IValidateable
    {
        [FormField(IsRequired:true)]
        public EntityHeader<EhChildModel> IsRequiredTopLevelProperty { get; set; }


        [FormField(IsRequired: false)]
        public EntityHeader<EhChildModel> NotRequired { get; set; }
    }

    public class EhChildModel : IValidateable
    {
        [FormField(IsRequired:true)]
        public string IsRequiredChildProperty { get; set; }


        [FormField(IsRequired: false)]
        public string IsNotRequiredProp { get; set; }

        [FormField(IsRequired:true)]
        public EntityHeader<EhGrandChildModel> GrandChild { get; set; }
    }

    public class EhGrandChildModel : IValidateable
    {

        [FormField(IsRequired: true)]
        public string IsRequiredGrandChildProperty { get; set; }


        [FormField(IsRequired: false)]
        public string IsNotRequiredProp { get; set; }
    }


    public class StringLengthModel : IValidateable
    {
        [FormField(MinLength:5)]
        public String MinStringLength_5 { get; set; }
        [FormField(MaxLength: 15)]
        public String MaxStringLength_15 { get; set; }
        [FormField(MinLength: 5, MaxLength: 15)]
        public String BetweenStringLength_5_15 { get; set; }
    }

    public class ValidatableObjectGraph : IValidateable
    {       
        public ValidationModel ChildModel { get; set; }
        public List<ValidationModel> ChildModels { get; set; }
    }

    public class RegExModel : IValidateable
    {
        [FormField(ValidationRegEx:"^[0-9a-z]{2,20}$", RegExValidationMessageResource:ValidationResources.Names.RegExMessage, FieldType:FieldTypes.Text, ResourceType: typeof(ValidationResources))]
        public string RegExValue_2_20_lower_case { get; set; }

        [FormField(FieldType:FieldTypes.Key, RegExValidationMessageResource: ValidationResources.Names.KeyIsBad, ResourceType:typeof(ValidationResources))]
        public string Key { get; set; }

    }
}
