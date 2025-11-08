// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cae2e38881a2c7179d3763e0c3dc343b9d2a3c18b79f131f1bd23c7172c277a3
// IndexVersion: 2
// --- END CODE INDEX META ---
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
