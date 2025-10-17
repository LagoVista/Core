// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e0fa90ec03a54bb39e4d71093642ea701d58a870ca1dfb0a6e6fd86e90f93712
// IndexVersion: 1
// --- END CODE INDEX META ---
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
