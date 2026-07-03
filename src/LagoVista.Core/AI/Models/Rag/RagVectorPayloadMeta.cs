using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagVectorPayloadMeta : RagCoreVectorPayloadMeta
    {
        public RagVectorPayloadMeta()
        {
            IsReference = false;
        }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Integer)]
        public int Priority { get; set; } = 3;


        public string BusinessDomain { get; set; }

        public string Audience { get; set; }

        public string Persona { get; set; }
    }
}
