using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagCodeVectorPayloadExtra : RagCoreVectorPayloadExtra
    {
        public int? LineStart { get; set; }

        public int? LineEnd { get; set; }

        public int? CharStart { get; set; }

        public int? CharEnd { get; set; }

        public string SymbolFullName { get; set; }

        public string SymbolName { get; set; }

        public string SymbolType { get; set; }

        public string SymbolContentUrl { get; set; }

    }
}
