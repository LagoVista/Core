// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6de5e71d1af70009784af79fb3e6c6b0cc455bd1ca75ff2f9a7a58d16d2c2866
// IndexVersion: 2
// --- END CODE INDEX META ---
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
