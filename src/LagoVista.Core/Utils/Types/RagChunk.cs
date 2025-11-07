namespace LagoVista.Core.Utils.Types
{
    public class RagChunk
    {
        public string PointId { get; set; }
        public string DocId { get; set; }
        public string SectionKey { get; set; }
        public int PartIndex { get; set; }     // 1-based
        public int PartTotal { get; set; }
        public string Title { get; set; }
        public string TextNormalized { get; set; }
        public int? LineStart { get; set; }    // optional pointer into raw text (1-based)
        public int? LineEnd { get; set; }      // optional pointer into raw text (inclusive)
        public int? CharStart { get; set; }
        public int? CharEnd { get; set; }
        public string Symbol { get; set; }
        public string SymbolType { get; set; }
        public float[] Vector { get; set; }

    }
}