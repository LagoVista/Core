using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface  IEmbedder
    {
        Task<InvokeResult<float[]>> EmbedAsync(string text, string embeddingModel = null);
    }
}
