using System.Collections.Generic;

namespace LagoVista.Core.AI.Models
{
    public class FileBundle
    {
        public string Root { get; set; } = ".";

        public List<FileBundleItem> Files { get; set; } = new List<FileBundleItem>();
    }
}
