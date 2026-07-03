namespace LagoVista.Core.AI.Models
{
    public sealed class RagEmbeddingSource
    {
        public string SourceField { get; set; }

        public string ReferenceType { get; set; }

        public int SourceIndex { get; set; }

        public int Priority { get; set; }

        public string Content { get; set; }
    }
}