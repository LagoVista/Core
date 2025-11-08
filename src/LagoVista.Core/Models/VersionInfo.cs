// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 40f771d010f646804326bed751184ea43b026b80008e740d7d09f9ebbb028493
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models
{
    public class VersionInfo
    {
        public int Major { get; set; }
        public int Minor { get; set; }        
        public long Build { get; set; }
        public int Revision { get; set; }
        public DateTime DateStamp { get; set; }
        public String ReleaseNotes { get; set; }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision}";
        }
    }
}
