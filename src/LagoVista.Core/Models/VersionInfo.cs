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
