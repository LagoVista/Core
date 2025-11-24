using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using LagoVista.Core.Validation;
using System.Collections.Generic;

namespace LagoVista.Core.AI.Interfaces
{
    /// <summary>
    /// IDX-0061 – Definition of IRagPayloadFactory
    ///
    /// Declares that an implementing type can construct a RagVectorPayload
    /// from its current state (previously supplied context, normalized text,
    /// metadata, etc.) without requiring additional parameters.
    /// </summary>
    public interface IRagPointFactory
    {
        /// <summary>
        /// Creates a fully-formed RagVectorPayload instance using the
        /// implementing object's current state.
        /// Implementations must not return null.
        /// </summary>
        /// <returns>A constructed RagVectorPayload wrapped in an invoke result.</returns>
        IEnumerable<InvokeResult<IRagPoint>> CreateIRagPoints();
    }
}
