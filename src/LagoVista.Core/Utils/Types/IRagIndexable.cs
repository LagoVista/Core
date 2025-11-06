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