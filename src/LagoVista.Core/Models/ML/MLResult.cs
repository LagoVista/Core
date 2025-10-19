// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7cb9caf77d15e9ede9ed430dadbf74ef027cda92840bceb5b208a5c414c69afa
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.Drawing;

namespace LagoVista.Core.Models.ML
{
    public class MLResult
    {
        public string LabelName { get; set; }
        public string LabelKey { get; set; }
        public double? Value { get; set; }
        public double Confidence { get; set; }
        public Rectangle BoundingRect { get; set; }
    }
}
