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
