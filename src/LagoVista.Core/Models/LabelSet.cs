using LagoVista.Core.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    public class LabelSet : EntityBase 
    {
        public LabelSet()
        {
            Labels = new List<Label>();
            Name = "labelset";
        }

        public List<Label> Labels { get; set; }

    }

    public class Label
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
    }
}
