using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagCoreVectorPayloadMeta : IRagVectorPayloadMeta
    {
        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string OrgNamespace { get; set; }

        [RagRequired]
        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string OrgId { get; set; }

        [RagRequired]
        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string DocId { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Boolean)]
        public bool IsReference { get; set; }


        [RagNotDefault("ContentTypeId must be specified and cannot be Unknown.")]
        [QdrantPayloadIndex(QdrantPayloadIndexKind.Integer)]
        public RagContentType ContentTypeId { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string Subtype { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public List<string> LabelSlugs { get; set; } = new List<string>();

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Integer)]
        public long IndexedUnix { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Boolean)]
        public bool HasIssues { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string Language { get; set; } = "en-US";

        public string EmbeddingRole { get; set; }


        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string StatusKey { get; set; }


        public string ParentPointId { get; set; }


        [QdrantPayloadIndex(QdrantPayloadIndexKind.Boolean)]
        public bool Deleted { get; set; }

        public string ContentType { get; set; } = "text/plain";
        public string SubtypeFlavor { get; set; }
        public string Title { get; set; }

        public string Stage { get; set; }

        [RagMinimum(1)]
        [RagDefaultValue(1, "IndexVersion was invalid and was defaulted to 1.")]
        public int IndexVersion { get; set; } = 1;

        [RagRequired]
        [RagDefaultValue("text-embedding-3-large", "EmbeddingModel was empty and was defaulted.")]
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";

        public string ContentHash { get; set; }

        public DateTime IndexedUtc { get; set; } = DateTime.UtcNow;

        public virtual void ValidateForIndex(InvokeResult result)
        {
            RagPayloadValidationResolver.ValidateAndNormalize(this, result);

            if (String.IsNullOrWhiteSpace(ContentType) && ContentTypeId != RagContentType.Unknown)
            {
                ContentType = ContentTypeId.ToString();
            }

            if (IndexedUtc == default)
            {
                IndexedUtc = DateTime.UtcNow;
                result.AddWarning("IndexedUtc was empty and was set to the current UTC time.");
            }

            IndexedUnix = new DateTimeOffset(IndexedUtc).ToUnixTimeSeconds();
        }
    }
}
