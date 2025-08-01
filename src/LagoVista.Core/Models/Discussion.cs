﻿using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
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

        /// <summary>
        /// We take a hash of the note, if it changed at all we resend the notification.
        /// </summary>
        public string NoteHash { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Discussion_Responses, IsRequired: false, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(LagoVistaCommonStrings))]
        public List<DiscussionResponse> Responses { get; set; } = new List<DiscussionResponse>();

        public bool Handled { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Open),
                nameof(Note),
                nameof(Responses)
            };
        }
    }

    [EntityDescription(LGVCommonDomain.CommonDomain, Resources.LagoVistaCommonStrings.Names.DiscussionResponse_Title, Resources.LagoVistaCommonStrings.Names.DiscussionResponse_Help,
        LagoVistaCommonStrings.Names.DiscussionResponse_Help, EntityDescriptionAttribute.EntityTypes.Discussion, typeof(LagoVistaCommonStrings), Icon: "icon-ae-chatting-2",
        FactoryUrl: "/api/discussion/response/factory")]
    public class DiscussionResponse
    {
        public DiscussionResponse()
        {
            Id = Guid.NewGuid().ToId();
            Timestamp = DateTime.UtcNow.ToJSONString();
        }
        public string Id { get; set; }
        public EntityHeader User { get; set; }
        public string Timestamp { get; set; }

        public bool Handled { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Discussion, IsRequired: true, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Note { get; set; }

        public string NoteHash { get; set; }

    }
}
