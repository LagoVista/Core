using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagEntityVectorPayloadExtra : RagCoreVectorPayloadExtra
    {
        public string EditorUrl { get; set; }

        public string PreviewUrl { get; set; }

        public string RestGETUrl { get; set; }

        public string RestPUTUrl { get; set; }
    }
}
