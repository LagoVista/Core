using System.Collections.Generic;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class QdrantPoint
    {
        public string Id { get; set; }

        public float[] Vector { get; set; }

        public Dictionary<string, object> Payload { get; set; }
    }
}
