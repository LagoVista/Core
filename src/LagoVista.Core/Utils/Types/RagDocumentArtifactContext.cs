namespace LagoVista.Core.Utils.Types
{
    public sealed class DocumentArtifactContext
    {
        public string Subtype { get; set; }          // e.g., "UserGuide", "FAQ", "EmailTemplate"
        public string Language { get; set; }         // override language if desired
        public string TitleOverride { get; set; }    // force a specific title for all chunks
    }
}
