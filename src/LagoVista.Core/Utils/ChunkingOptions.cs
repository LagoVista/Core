// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2ece7b273635a21be9348ee93f184635a1b3b12a394542966ef5ded88ce281ee
// IndexVersion: 2
// --- END CODE INDEX META ---
namespace LagoVista.Core.Utils
{
    public sealed class ChunkingOptions
    {
        public int TargetTokensPerChunk { get; set; } = 700;
        public int OverlapTokens { get; set; } = 80;
        public bool IncludeFrontMatter { get; set; } = true;
        public string VersionOrDate { get; set; } = null; // e.g. "2025-11-03" or commit SHA
        public string RawFileExtension { get; set; } = "txt"; // default if not inferred
    }
}