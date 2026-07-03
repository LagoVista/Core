using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagEntityVectorPayloadMeta : RagCoreVectorPayloadMeta
    {
        public RagEntityVectorPayloadMeta() 
        {
            ContentTypeId = RagContentType.Entity;
        }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string BusinessDomain { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string EntityType { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string EntityId { get; set; }

        public string SemanticId { get; set; }
    }
}
