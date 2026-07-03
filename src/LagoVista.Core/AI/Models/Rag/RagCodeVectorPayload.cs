using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagCodeVectorPayload : RagVectorPayloadBase<RagCodeVectorPayloadMeta, RagCodeVectorPayloadExtra>
    {
        public override JObject Serialize()
        {
            return JObject.FromObject(this);
        }

    }
}
