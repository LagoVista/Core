using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Resources;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Networking.Models
{

    [EntityDescription(LGVCommonDomain.CommonDomain, NetworkingResources.Names.Subscription_Title, NetworkingResources.Names.Subscription_Description,
    NetworkingResources.Names.Subscription_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(NetworkingResources),
        FactoryUrl: "/api/pipeline/admin/listener/subscription/factory")]
    public class MQTTSubscription : IDescriptionEntity, IFormDescriptor
    {
        public MQTTSubscription()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: NetworkingResources.Names.Subscription_Topic, HelpResource:NetworkingResources.Names.Subscription_Topic_Help,  FieldType: FieldTypes.Text, ResourceType: typeof(NetworkingResources), IsRequired: true)]
        public string Topic { get; set; }

        [FormField(LabelResource: NetworkingResources.Names.Subscription_QOS, HelpResource: NetworkingResources.Names.Subscription_QOS_Help, FieldType: FieldTypes.Picker, WaterMark: NetworkingResources.Names.Subscription_QOS_Select, EnumType: typeof(QOS), ResourceType: typeof(NetworkingResources), IsRequired: true)]
        public EntityHeader<QOS> QOS { get; set; }


        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false)]
        public string Description { get; set; }


        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                 nameof(Topic),
                 nameof(QOS),
                 nameof(Description),
            };
        }
    }
}
