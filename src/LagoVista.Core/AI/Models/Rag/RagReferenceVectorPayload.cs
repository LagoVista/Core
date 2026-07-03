using LagoVista.Core.Validation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagReferenceVectorPayload : RagVectorPayloadBase<RagReferenceVectorPayloadMeta, RagReferenceVectorPayloadExtra>
    {
        public static RagReferenceVectorPayload FromDictionary(IDictionary<string, object> source)
        {
            return FromDictionary<RagReferenceVectorPayload>(source);
        }

        public static RagReferenceVectorPayload FromPrimary(RagEntityVectorPayload primary)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            var payload = new RagReferenceVectorPayload
            {
                Meta = RagPayloadMapper.CopySharedProperties<RagEntityVectorPayloadMeta, RagReferenceVectorPayloadMeta>(primary.Meta),
                Extra = RagPayloadMapper.CopySharedProperties<RagEntityVectorPayloadExtra, RagReferenceVectorPayloadExtra>(primary.Extra)
            };

            payload.Meta.IsReference = true;
            return payload;
        }

        public static string BuildPointId(string entityType, string entityId, string sourceField, int sourceIndex)
        {
            if (String.IsNullOrWhiteSpace(entityType))
            {
                throw new ArgumentException("Entity type is required.", nameof(entityType));
            }

            if (String.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentException("Entity ID is required.", nameof(entityId));
            }

            if (String.IsNullOrWhiteSpace(sourceField))
            {
                throw new ArgumentException("Source field is required.", nameof(sourceField));
            }

            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Source index cannot be less than zero.");
            }

            return $"{Slug(entityType, "unknown")}:{Slug(entityId, "unknown")}:{Slug(sourceField, "unknown")}:{sourceIndex}";
        }

        public override JObject Serialize()
        {
            return JObject.FromObject(this);
        }

        public override string ToString()
        {
            return $"{Meta.OrgNamespace}/{Meta.DocId} reference={Meta.IsReference}";
        }
    }
}
