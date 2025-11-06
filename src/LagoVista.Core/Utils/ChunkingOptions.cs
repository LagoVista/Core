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