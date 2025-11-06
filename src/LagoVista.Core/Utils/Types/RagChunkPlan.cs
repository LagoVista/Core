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