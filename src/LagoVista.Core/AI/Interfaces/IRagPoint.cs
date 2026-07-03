using LagoVista.Core.AI.Models.Rag;
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
        public IRagVectorPayload Payload { get; set; }
        public byte[] Contents { get; set; }
        public byte[] FinderSnippet { get; set; }
    }

    public interface IRagPoint<TPayload> where TPayload : IRagVectorPayload
    {
        string PointId { get; set; }
        public float[] Vector { get; set; }
        public TPayload Payload { get; set; }
        public byte[] Contents { get; set; }
        public byte[] FinderSnippet { get; set; }
    }
}
