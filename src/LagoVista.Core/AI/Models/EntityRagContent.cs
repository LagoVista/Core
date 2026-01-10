using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public class EntityRagContent
    {
        public RagVectorPayload Payload { get; set; }
        public bool SummarizeModelForEmbeddings { get; set; }
        public string EmbeddingContent {get; set;}
        public string ModelDescription { get; set; }
        public string HumanDescription { get; set; }
        public string Issues { get; set; }
    }
}
