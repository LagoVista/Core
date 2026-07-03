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

        public int? LineStart { get; set; }

        public int? LineEnd { get; set; }

        public int? CharStart { get; set; }

        public int? CharEnd { get; set; }

        public string SymbolFullName { get; set; }

        public string SymbolName { get; set; }

        public string SymbolType { get; set; }

        public string SymbolContentUrl { get; set; }

        public string PrimaryEntity { get; set; }

        public string HtmlAnchor { get; set; }

        public List<int> PdfPages { get; set; }

        public string Repo { get; set; }

        public string RepoBranch { get; set; }

        public string CommitSha { get; set; }

        public string Path { get; set; }

        public int? StartLine { get; set; }

        public int? EndLine { get; set; }

        public string EditorUrl { get; set; }

        public string PreviewUrl { get; set; }

        public string RestGETUrl { get; set; }

        public string RestPUTUrl { get; set; }

        public string SampleKindText { get; set; }

        public string ArtifactTypeText { get; set; }

        public string ArtifactText { get; set; }

        public string VtmMeetingText { get; set; }

        public string SopExecutionText { get; set; }

        public string ScopeTypeText { get; set; }

        public string ScopeText { get; set; }
    }
}
