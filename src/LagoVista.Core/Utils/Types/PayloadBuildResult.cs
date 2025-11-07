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
