// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8e422be2a7dad5802d4d4f1f485c34466c103570017a40b9d51db05781ec84b6
// IndexVersion: 2
// --- END CODE INDEX META ---
namespace LagoVista.Core.Utils.Types
{
    public sealed class DocumentArtifactContext
    {
        public string Subtype { get; set; }          // e.g., "UserGuide", "FAQ", "EmailTemplate"
        public string Language { get; set; }         // override language if desired
        public string TitleOverride { get; set; }    // force a specific title for all chunks
    }
}
