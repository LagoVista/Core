// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1f0032deb9e7d63b36c6d9ad6f64a52c61a1ca9d6b5af7cea19a62ab146310c4
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Utils.Types;
using LagoVista.Core.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IRagServices
    {
        float[] GetEmbedingsAsync(IAIAgentContext agentContext, string text);
        Task<InvokeResult> AddTextContentAsync(IAIAgentContext agentContext, string path, string fileName, string content, string contentType);
        Task UpsertInBatchesAsync(IAIAgentContext agentContext, IReadOnlyList<PayloadBuildResult> points, int vectorDims, int? maxPerBatch = null, CancellationToken ct = default);
        Task RemoveStaleVectorsAsync(IAIAgentContext agentContext, string docId, CancellationToken ct = default);
    }

    public class RAGServicesProvideer
    {
        public static IRagServices Instance { get; }
    }
}
