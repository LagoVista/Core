using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    /// <summary>
    /// Represents a single facet key/value pair in the meta data registry
    /// (e.g., Kind=SourceCode, SubKind=Model, ChunkFlavor=Raw).
    /// </summary>
    public sealed class MetaFacet
    {
        /// <summary>
        /// Facet key name, e.g. "Kind", "SubKind", "ChunkFlavor", "Repo", etc.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Facet value for the given key, e.g. "SourceCode", "Model", "Raw".
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents a unique combination of facets discovered during an indexing run
    /// for a particular org/project/component.
    /// </summary>
    public sealed class MetaRegistryEntry
    {
        /// <summary>
        /// Organization identifier (must match OrgId used in vector payloads).
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// Project identifier (must match ProjectId used in vector payloads).
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Logical name of the indexer or component producing this entry,
        /// e.g. "ServerCodeIndexer", "NuvOS.DesignSystem.Indexer".
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// Ordered list of facet key/value pairs that define this registry entry.
        /// For example:
        ///   [ Kind=SourceCode ]
        ///   [ Kind=SourceCode, SubKind=Model ]
        ///   [ Kind=SourceCode, SubKind=Model, ChunkFlavor=Raw ]
        /// </summary>
        public List<MetaFacet> Facets { get; set; } = new List<MetaFacet>();
    }
}
