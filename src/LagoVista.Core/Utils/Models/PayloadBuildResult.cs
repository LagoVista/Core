// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b36d2140b6b9c131f5fc8fd550e5c121ec020d13a6ef24dc8c3c1fb295874cbf
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    public sealed class PayloadBuildResult
    {
        public string PointId { get; set; }
        public RagVectorPayload Payload { get; set; }
        public string TextForEmbedding { get; set; }  // normalized chunk text
        public int EstimatedTokens { get; set; }      // coarse estimate
        public float[] Vector { get; set; }
    }
}
