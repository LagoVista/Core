using LagoVista.Core.Validation;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagReferenceVectorPayloadMeta : RagCoreVectorPayloadMeta
    {
        public RagReferenceVectorPayloadMeta()
        {
            IsReference = true;
            ContentTypeId = RagContentType.Entity;
        }

        public override void ValidateForIndex(InvokeResult result)
        {
            base.ValidateForIndex(result);

            if (!IsReference)
            {
                IsReference = true;
                result.AddWarning("IsReference was false and was normalized to true.");
            }
        }
    }
}
