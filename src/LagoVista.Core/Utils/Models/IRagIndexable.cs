// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4db408500634e8616ec04c11951ed0e2279d43ddad7ff24c7cd4a1fd67e0edc3
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System.Collections.Generic;
using System.Xml;

namespace LagoVista.Core.Utils.Types
{
    // RagChunkBuilder.cs
    // RagChunkBuilder.cs (concise)

    public interface IRagIndexable
    {
        EntityHeader OwnerOrganization { get; }
        string Id { get; }
        string ContentSubtype { get; }

        string GetFrontMatter();
        IEnumerable<IndexSection> GetBodySections();

        IEnumerable<string> GetLabelSlugs();
        string Language { get; }
        int Priority { get; }
        string Audience { get; }
        string Persona { get; }
        string Stage { get; }
    }
}