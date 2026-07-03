using System;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class QdrantPayloadIndexSpec
    {
        public QdrantPayloadIndexSpec(string path, QdrantPayloadIndexKind kind)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Kind = kind;
        }

        public string Path { get; }

        public QdrantPayloadIndexKind Kind { get; }
    }
}
