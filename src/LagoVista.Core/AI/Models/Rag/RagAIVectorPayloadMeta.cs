using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagAIVectorPayloadMeta : RagCoreVectorPayloadMeta
    {
        public RagAIVectorPayloadMeta() 
        {
            ContentTypeId = RagContentType.Ai;
        }

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
