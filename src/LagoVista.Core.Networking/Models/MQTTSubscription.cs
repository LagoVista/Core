using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Resources;

namespace LagoVista.Core.Networking.Models
{
    public class MQTTSubscription
    {

        [FormField(LabelResource: NetworkingResources.Names.Subscription_Topic, HelpResource:NetworkingResources.Names.Subscription_Topic_Help,  FieldType: FieldTypes.Text, ResourceType: typeof(NetworkingResources), IsRequired: true)]
        public string Topic { get; set; }

        [FormField(LabelResource: NetworkingResources.Names.Subscription_QOS, HelpResource: NetworkingResources.Names.Subscription_QOS_Help, FieldType: FieldTypes.Picker, WaterMark: NetworkingResources.Names.Subscription_QOS_Select, EnumType: typeof(QOS), ResourceType: typeof(NetworkingResources), IsRequired: true)]
        public EntityHeader<QOS> QOS { get; set; }
    }
}
