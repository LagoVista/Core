namespace LagoVista.Core.AI.Interfaces
{
    /// <summary>
    /// IDX-0062 – IRagableEntity marker interface.
    ///
    /// Combines:
    /// - IDX-0060 IEmbeddable (normalized text, token estimate, embedding vector)
    /// - IDX-0061 IRagPayloadFactory (CreateRagPayload method)
    ///
    /// Any entity implementing this interface is considered RAG ready:
    /// it can be embedded and can produce a RagVectorPayload for indexing.
    /// </summary>
    public interface IRagableEntity : IEmbeddable, IRagPointFactory
    {
    }
}
