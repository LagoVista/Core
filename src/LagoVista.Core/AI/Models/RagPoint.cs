using LagoVista.Core.AI.Interfaces;
using LagoVista.Core.AI.Models.Rag;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public class RagPoint : IRagPoint
    {
        public string PointId { get; set; }
        public float[] Vector { get; set; }
        public IRagVectorPayload Payload { get; set; }
        public byte[] FinderSnippet { get; set; }
        
        public byte[] Contents { get; set; }

        public JObject Serialize() => new JObject
        {
            ["id"] = PointId,
            ["vector"] = new JArray(Vector),
            ["payload"] = Payload.Serialize(),
        };
    }

    public class RagPoint<TPayload>  where TPayload : IRagVectorPayload
    {
        public string PointId { get; set; }
        public float[] Vector { get; set; }
        public TPayload Payload { get; set; }
        public byte[] FinderSnippet { get; set; }

        public byte[] Contents { get; set; }
 
        public JObject Serialize() => new JObject
        {
            ["id"] = PointId,
            ["vector"] = new JArray(Vector),
            ["payload"] = Payload.Serialize(),
        };
    }
}
