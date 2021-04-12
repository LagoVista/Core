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
