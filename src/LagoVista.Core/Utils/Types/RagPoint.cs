using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    public sealed class QdrantPoint
    {
        public object Id { get; set; }                       // string or int
        public float[] Vector { get; set; }                  // embedding
        public System.Collections.Generic.Dictionary<string, object> Payload { get; set; }
    }
}
