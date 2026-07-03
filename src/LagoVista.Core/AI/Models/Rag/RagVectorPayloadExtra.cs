using System.Collections.Generic;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagVectorPayloadExtra : RagCoreVectorPayloadExtra
    {
        public string FullDocumentBlobUri { get; set; }

        public string SourceSliceBlobUri { get; set; }

        public string DescriptionBlobUri { get; set; }

        public string BlobVersionId { get; set; }

        public string SourceSha256 { get; set; }

        public string HtmlAnchor { get; set; }

        public List<int> PdfPages { get; set; }


        public string SampleKindText { get; set; }


        public string VtmMeetingText { get; set; }

        public string SopExecutionText { get; set; }

        public string ScopeTypeText { get; set; }

        public string ScopeText { get; set; }
    }
}
