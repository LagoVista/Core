using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagCodeVectorPayloadExtra : RagCoreVectorPayloadExtra
    {
        public string FullDocumentBlobUri { get; set; }

        public int? LineStart { get; set; }

        public int? LineEnd { get; set; }

        public int? CharStart { get; set; }

        public int? CharEnd { get; set; }

        public string SymbolFullName { get; set; }

        public string SymbolName { get; set; }

        public string SymbolType { get; set; }

        public string SymbolContentUrl { get; set; }


        public string Repo { get; set; }

        public string RepoBranch { get; set; }

        public string CommitSha { get; set; }

        public string Path { get; set; }

        public int? StartLine { get; set; }

        public int? EndLine { get; set; }
        public string SourceSliceBlobUri { get; set; }
        public string DescriptionBlobUri { get; set; }
        public string SourceSha256 { get; set; }

    }
}
