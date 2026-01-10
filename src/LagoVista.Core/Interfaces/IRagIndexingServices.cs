// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1f0032deb9e7d63b36c6d9ad6f64a52c61a1ca9d6b5af7cea19a62ab146310c4
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.AI.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Utils.Types;
using LagoVista.Core.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IRagIndexingServices
    {
        float[] GetEmbedingsAsync(IAIAgentContext agentContext, string text);
        Task UpsertInBatchesAsync(IAIAgentContext agentContext, IReadOnlyList<PayloadBuildResult> points, int vectorDims, int? maxPerBatch = null, CancellationToken ct = default);
        Task RemoveStaleVectorsAsync(IAIAgentContext agentContext, string docId, CancellationToken ct = default);
        Task<InvokeResult> IndexAsync(IEntityBase entity);
        Task<InvokeResult> RemoveIndexAsync(string orgId, string docId);
    }
}
