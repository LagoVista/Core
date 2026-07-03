using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public interface IRagVectorPayload
    {
        IReadOnlyList<QdrantPayloadIndexSpec> Indexes { get; }
    }
}
