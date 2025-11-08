// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e14e7bc218fddcd9cd1859e7a5bbde81c64a82e8db05ce5960e6496e039e6e21
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    /// <summary>
    /// Describes the code artifact the chunks came from.
    /// </summary>
    public sealed class CodeArtifactContext
    {
        public string Repo { get; set; }             // e.g., "github.com/acme/nuviot"
        public string RepoBranch { get; set; }       // e.g., "main"
        public string CommitSha { get; set; }        // short or full SHA
        public string Path { get; set; }             // repo-relative path, e.g., "src/Services/OrderService.cs"
        public string Subtype { get; set; }          // e.g., "CSharp", "TypeScript", "SQL"
        public string Language { get; set; }         // optional content language tag for code docs/comments
        public string TitleOverride { get; set; }    // if you want a specific title per chunk (else we pick)
    }
}
