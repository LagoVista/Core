using LagoVista.Core.Interfaces;
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
        void CreateEmbeddings(IEmbedder embeddingService);
    }
}
