using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagCodeVectorPayloadMeta : RagCoreVectorPayloadMeta
    {
        public RagCodeVectorPayloadMeta()
        {
            ContentTypeId = RagContentType.SourceCode;
        }


        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SemanticId { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SysDomain { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SysLayer { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SysRole { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string ProjectId { get; set; }

        public string SourceSystem { get; set; }

        public string SourceObjectId { get; set; }

        public int? OverlapTokens { get; set; }


        public string SectionKey { get; set; }

        public int PartIndex { get; set; }

        public int PartTotal { get; set; }

        public int? ChunkSizeTokens { get; set; }
        public int? ContentLenChars { get; set; }


        public List<string> LabelIds { get; set; }

        public override void ValidateForIndex(InvokeResult result)
        {
            base.ValidateForIndex(result);

            IsReference = false;

            if (String.IsNullOrWhiteSpace(SectionKey))
            {
                SectionKey = "body";
                result.AddWarning("SectionKey was empty and was defaulted to body.");
            }

            if (PartIndex < 1)
            {
                PartIndex = 1;
                result.AddWarning("PartIndex was less than 1 and was normalized to 1.");
            }

            if (PartTotal < PartIndex)
            {
                PartTotal = PartIndex;
                result.AddWarning("PartTotal was less than PartIndex and was normalized to match PartIndex.");
            }

            if (String.IsNullOrWhiteSpace(SemanticId) && !String.IsNullOrWhiteSpace(DocId))
            {
                SemanticId = RagVectorPayload.BuildSemanticId(DocId, SectionKey, PartIndex);
                result.AddWarning("SemanticId was generated from DocId, SectionKey, and PartIndex.");
            }
        }

    }
}
