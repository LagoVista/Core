using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Interfaces
{
    public interface IRagPoint
    {
        string PointId { get; set; }
        public float[] Vector { get; set; }
        public RagVectorPayload Payload { get; set; }
        public byte[] Contents { get; set; }
        public byte[] FinderSnippet { get; set; }

    }
}
