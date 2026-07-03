using LagoVista.Core.Validation;
using System;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagReferenceVectorPayloadExtra : RagCoreVectorPayloadExtra
    {
        public override void ValidateForIndex(InvokeResult result)
        {
            base.ValidateForIndex(result);

            if (String.IsNullOrWhiteSpace(ShortSummary))
            {
                result.AddWarning("ShortSummary is not set. The reference may require canonical lookup before it can be meaningfully evaluated.");
            }
        }
    }
}
