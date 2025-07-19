using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    [EntityDescription(LGVCommonDomain.CommonDomain, Resources.LagoVistaCommonStrings.Names.Discussion_Title, Resources.LagoVistaCommonStrings.Names.Discussion_Help,
            LagoVistaCommonStrings.Names.Discussion_Help, EntityDescriptionAttribute.EntityTypes.Discussion, typeof(LagoVistaCommonStrings), Icon: "icon-ae-chatting-2",
            FactoryUrl: "/api/discussion/factory")]
    public class Discussion : IFormDescriptor
    {
        public Discussion()
        {
            Id = Guid.NewGuid().ToId();
            Timestamp = DateTime.UtcNow.ToJSONString();
        }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Open,  FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Open { get; set; } = true;

        public string Id { get; set; }
        public EntityHeader User { get; set; }

        public string Timestamp { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Discussion, IsRequired: true, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Note { get; set; }


        [FormField(LabelResource: LagoVistaCommonStrings.Names.Discussion_Response, IsRequired: false, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Response { get; set; }


        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Open),
                nameof(Note),
                nameof(Response)
            };
        }
    }
}
