using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagArtifactVectorPayloadExtra : RagCoreVectorPayloadExtra
    {
        public string ArtifactTypeText { get; set; }

        public string ArtifactText { get; set; }

        public string VtmMeetingText { get; set; }
        public string SopExecutionText { get; set; }
        public string SampleKindText { get; set; }
        public string ScopeTypeText { get; set; }
        public string ScopeText { get; set; }
    }
}
