using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Interfaces
{
    public interface IRagPoint
    {
        string PointId { get; set; }
        public float[] Vector { get; set; }
        System.Collections.Generic.Dictionary<string, object> Payload { get; set; }
    }
}
