// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 06ae84061077aa0cf9d0a8b15fb64812ed5dc045899c3f0756ef554a1c98983e
// IndexVersion: 2
// --- END CODE INDEX META ---
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
