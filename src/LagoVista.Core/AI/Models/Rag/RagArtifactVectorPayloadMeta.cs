using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagArtifactVectorPayloadMeta : RagCoreVectorPayloadMeta
    {
        public RagArtifactVectorPayloadMeta()
        {
            PayloadFamily = RagPayloadFamily.Artifact;
            ContentTypeId = RagContentType.ProducedArtifact;
        }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string ScopeType { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string ScopeId { get; set; }

        public string ArtifactArchetype { get; set; }

        [QdrantPayloadIndex(QdrantPayloadIndexKind.Keyword)]
        public string ArtifactArchetypeKey { get; set; }

        public string VirtualTeamMemberId { get; set; }

        public string EssentialJobActivityId { get; set; }

        public string ArtifactTypeId { get; set; }

        public string ArtifactId { get; set; }

        public string SopWorkItemId { get; set; }

        public bool IsSample { get; set; }

        public string SampleKindId { get; set; }

        public string VtmMeetingId { get; set; }

        public string SopExecutionId { get; set; }
    }
}
