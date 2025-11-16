// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cd7bb30d14147a3a20c0e4c6b511aac0da03df45107c93cb6720fb76d48d7b99
// IndexVersion: 2
// --- END CODE INDEX META ---
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
        public int EstimatedTokens { get; set; }
    }
}