// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f6cb15b302c8cbf0b0e4e23bd3618e033b3962ebaedc5b562987b866aea54084
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Cloning
{
    public class CloneRequest
    {
        public string OriginalId { get; set; }
        public string NewName { get; set; }
        public string NewKey { get; set; }
    }
}
