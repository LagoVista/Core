// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1700856233c657221856456605b4dad93d52d6bdec6747dedc13f953e2e8ef5b
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Models
{
    public class File
    {
        public String Directory { get; set; }
        public String FileName { get; set; }
    }

    public class Folder
    {
        public String FullPath { get; set; }
        public String Name
        {
            get
            {
                var parts = FullPath.Split('\\');

                return parts.Last();
            }
        }
    }
}
