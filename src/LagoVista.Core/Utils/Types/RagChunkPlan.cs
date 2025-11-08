// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4e52aa21114f4dab880b4cf94ff9d49f5c696bb86a49412edc76bbc745a77ad7
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Collections.Generic;

namespace LagoVista.Core.Utils.Types
{
    public sealed class RagChunkPlan
    {
        public string DocId { get; set; }
        public IReadOnlyList<RagChunk> Chunks { get; set; }
        public RawArtifact Raw { get; set; }
    }
}