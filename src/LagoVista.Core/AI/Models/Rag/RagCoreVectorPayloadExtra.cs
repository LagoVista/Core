using LagoVista.Core.Validation;
using System;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagCoreVectorPayloadExtra : IRagVectorPayloadExtra
    {
        public string ShortSummary { get; set; }

        public string ModelContentUrl { get; set; }

        public string HumanContentUrl { get; set; }

        public string IssuesContentUrl { get; set; }

        public virtual void ValidateForIndex(InvokeResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }
        }
    }
}
