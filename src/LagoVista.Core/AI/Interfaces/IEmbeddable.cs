using System;

namespace LagoVista.Core.AI.Interfaces
{
    /// <summary>
    /// IDX-0060 – Represents an artifact that can be embedded into a vector store.
    /// Provides normalized text, an estimated token count, and an embedding vector.
    ///
    /// This interface is intentionally minimal so it can be reused across many DDR-defined
    /// artifact types (chunks, descriptions, summaries, etc.).
    /// </summary>
    public interface IEmbeddable
    {
        /// <summary>
        /// Canonical normalized text used during embedding.
        /// Must be non-null, deterministic, and pre-cleaned for the target model.
        /// </summary>
        string NormalizedText { get; }

        /// <summary>
        /// Approximate token count of the normalized text.
        /// Used for budgeting, batching, and cost estimation.
        /// </summary>
        int EstimatedTokens { get; }

        /// <summary>
        /// Embedding vector assigned after the embedding service runs.
        /// May be null until populated by the embedding pipeline.
        /// </summary>
        float[] EmbeddingVectors { get; set; }
    }
}
