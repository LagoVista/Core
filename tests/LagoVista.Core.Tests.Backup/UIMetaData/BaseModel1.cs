using LagoVista.Core.Attributes;
using LagoVista.Core.Tests.Resources.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.UIMetaData
{
    public class BaseModel1
    {

        [FormField(LabelResource: MetaDataResources.Names.BaseField1, ResourceType: typeof(MetaDataResources), WaterMark: MetaDataResources.Names.BaseField1_Help, HelpResource: MetaDataResources.Names.BaseField1_Help, IsRequired: true, ReqMessageResource: MetaDataResources.Names.BaseField1_Requied)]
        public String BaseField { get; set; }

    }
}
