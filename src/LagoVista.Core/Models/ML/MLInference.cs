using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.ML
{
    public class MLInference
    {
        public string ModelName { get; set; }
        public string ModelRevision { get; set; }
        public string DeviceUniqueId { get; set; }
        public string PemId { get; set; }
        public string DateStamp { get; set; }

        public List<MLResult> Results {get; set;}
    }
}
