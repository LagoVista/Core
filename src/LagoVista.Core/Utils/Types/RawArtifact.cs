namespace LagoVista.Core.Utils.Types
{
    public sealed class RawArtifact
    {
        public string MimeType { get; set; }           // e.g., "text/markdown", "text/html"
        public string SuggestedBlobPath { get; set; }  // /{org}/{project}/{subtype}/{docId}/{ver}/source.ext
        public string SourceSha256 { get; set; }
        public bool IsText { get; set; }             // if true, Text is set; else Bytes/Stream provided elsewhere
        public string Text { get; set; }               // small docs only
    }
}