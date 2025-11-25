using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IEmbedder
    {
        Task<InvokeResult<EmbeddingResult>> EmbedAsync(string text, int? estimatedTokens = null, string embeddingModel = null);
    }

    public class EmbeddingResult
    {
        public EmbeddingResult(float[] vector, string embeddingModel)
        {
            Vector = vector;
            EmbeddingModel = embeddingModel;
        }

        public float[] Vector { get; }
        public string EmbeddingModel { get; }
    }
}
