using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    [EntityDescription(Domains.CoreDomainName, LagoVistaCommonStrings.Names.EntityOwnershipPoint_Title, LagoVistaCommonStrings.Names.EntityOwnershipPoint_Help,
    LagoVistaCommonStrings.Names.EntityOwnershipPoint_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(LagoVistaCommonStrings),
    Icon: "icon-ae-target", FactoryUrl: "/api/core/ownershippoint/factory")]
    public class EntityOwnershipPoint : IFormDescriptor
    {
        public EntityOwnershipPoint()
        {
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NormalizedId32 Id { get; set; } = NormalizedId32.Factory();

        [FormField(LabelResource: LagoVistaCommonStrings.Names.EntityOwnershipPoint_Action, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings),
            IsRequired: true, AiChatPrompt: "Use a concise business action of one to three words. Prefer common ownership verbs such as Create, Produce, Review, Approve, Maintain, Reconcile, Govern, Monitor, Coordinate, or Deliver when they accurately fit. Avoid synonyms that add no meaningful distinction, such as Build instead of Create or Examine instead of Review.")]
        public string Action { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.EntityOwnershipPoint_Subject, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings),
            IsRequired: true, AiChatPrompt: "Identify the business subject being owned using one to three words. Name the responsibility area, work product, process, or outcome rather than another entity.")]
        public string Subject { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.EntityOwnershipPoint_Summary, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings),
            IsRequired: true, AiChatPrompt: "Summarize the accountability boundary in fewer than twelve words. Explain what ownership means without simply repeating the action and subject.")]
        public string Summary { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>() { nameof(Action), nameof(Subject), nameof(Summary) };
        }
    }

    public interface IOwnershipPoints
    {
        List<EntityOwnershipPoint> OwnershipPoints { get; set; }
    }
}
