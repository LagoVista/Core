using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public class SourceRef
    {
        public string Tag { get; set; } = "";
        public string Path { get; set; } = "";
        public string FileName { get; set; } = "";
        public int Start { get; set; }
        public int End { get; set; }
        public string Excerpt { get; set; }
        public string Symbol { get; set; }
        public string SymbolType { get; set; }
    }
}
