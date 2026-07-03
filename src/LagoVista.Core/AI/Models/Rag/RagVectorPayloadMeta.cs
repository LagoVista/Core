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

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string ProjectId { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SemanticId { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SysDomain { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SysLayer { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string SysRole { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Integer)]
        public int Priority { get; set; } = 3;

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public List<string> LabelIds { get; set; } = new List<string>();

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Integer)]
        public long? UpdatedUnix { get; set; }


        public string ParentPointId { get; set; }

        public string BusinessDomainArea { get; set; }


        public string SectionKey { get; set; }

        public int PartIndex { get; set; }

        public int PartTotal { get; set; }

        public string Language { get; set; }


        public string Audience { get; set; }

        public string Persona { get; set; }


        public int? ChunkSizeTokens { get; set; }

        public int? OverlapTokens { get; set; }

        public int? ContentLenChars { get; set; }

        public DateTime? UpdatedUtc { get; set; }


        public string SourceSystem { get; set; }

        public string SourceObjectId { get; set; }

        public string VirtualTeamMemberId { get; set; }

        public string EssentialJobActivityId { get; set; }

        public string ArtifactTypeId { get; set; }

        public string ArtifactId { get; set; }

        public string SopWorkItemId { get; set; }

        public bool IsSample { get; set; }

        public string SampleKindId { get; set; }

        public string VtmMeetingId { get; set; }

        public string SopExecutionId { get; set; }

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

            UpdatedUnix = UpdatedUtc.HasValue ? new DateTimeOffset(UpdatedUtc.Value).ToUnixTimeSeconds()
                : (long?)null;
        }
    }
}
