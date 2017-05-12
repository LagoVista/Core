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
    public class ValidationModel : IValidateable
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

    public class AuditableModel : IAuditableEntity, IValidateable
    {
        public EntityHeader CreatedBy { get; set; }

        public string CreationDate { get; set; }

        public EntityHeader LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }
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

    public class StringLengthModel : IValidateable
    {
        [FormField(MinLength:5)]
        public String MinStringLength_5 { get; set; }
        [FormField(MaxLength: 15)]
        public String MaxStringLength_15 { get; set; }
        [FormField(MinLength: 5, MaxLength: 15)]
        public String BetweenStringLength_5_15 { get; set; }
    }
}
