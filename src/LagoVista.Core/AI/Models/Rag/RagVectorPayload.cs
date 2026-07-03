using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagVectorPayload : RagVectorPayloadBase<RagVectorPayloadMeta, RagVectorPayloadExtra>
    {
        public override string ToString()
        {
            var contentType = !String.IsNullOrWhiteSpace(Meta.ContentType)
                ? Meta.ContentType
                : Meta.ContentTypeId != RagContentType.Unknown ? Meta.ContentTypeId.ToString() : "Unknown";

            return $"{Meta.OrgNamespace}/{Meta.DocId} ({contentType})";
        }

        public static RagVectorPayload FromDictionary(IDictionary<string, object> source)
        {
            return FromDictionary<RagVectorPayload>(source);
        }

        public static string BuildSemanticId(string docId, string sectionKey, int partIndex)
        {
            if (String.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException("Document ID is required.", nameof(docId));
            }

            if (String.IsNullOrWhiteSpace(sectionKey))
            {
                sectionKey = "body";
            }

            if (partIndex < 1)
            {
                partIndex = 1;
            }

            return $"{docId}:sec:{Slug(sectionKey, "body")}#p{partIndex}";
        }

        public static string BuildPointId(string docId, string sectionKey, int partIndex)
        {
            return BuildSemanticId(docId, sectionKey, partIndex);
        }

        public override JObject Serialize()
        {
            return JObject.FromObject(this);
        }
    }
}
