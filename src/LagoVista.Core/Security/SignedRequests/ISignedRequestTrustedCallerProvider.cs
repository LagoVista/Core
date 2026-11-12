using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    /// <summary>
    /// Explicit receiver-side trust: a caller AppKey and the environment that
    /// registered its public verification key. Never sourced from request headers.
    /// </summary>
    public interface ISignedRequestTrustedCallerProvider
    {
        IReadOnlyCollection<SignedRequestTrustedCaller> GetTrustedCallers();
    }

    public sealed class SignedRequestTrustedCaller
    {
        public string AppKey { get; set; }
        public string Environment { get; set; }
    }
}
