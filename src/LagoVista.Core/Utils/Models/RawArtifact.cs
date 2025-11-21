// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c0b22e61d179cd0ce10aed56bedad7dd2aeae07138bfd4258ce75ca62c2ab04d
// IndexVersion: 2
// --- END CODE INDEX META ---
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