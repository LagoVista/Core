using LagoVista.Core.AI.Models.Rag;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.AI.Models
{
    public class EntityRagContent
    {
        public RagVectorPayload Payload { get; set; }

        public IReadOnlyList<EntityRagReferenceContent> ReferenceContents { get; set; } = Array.Empty<EntityRagReferenceContent>();

        public bool SummarizeModelForEmbeddings { get; set; }

        public string EmbeddingContent { get; set; }

        public string ModelContent { get; set; }

        public string HumanContent { get; set; }

        public string Issues { get; set; }
    }

    public sealed class EntityRagReferenceContent
    {
        public string PointId { get; set; }

        public string EmbeddingContent { get; set; }

        public RagReferenceVectorPayload Payload { get; set; }
    }
}