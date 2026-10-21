// --- BEGIN CODE INDEX META (do not edit) ---
// IndexVersion: 2
// --- END CODE INDEX META ---

namespace LagoVista.Core.Models
{
    /// <summary>
    /// Common optimistic-concurrency update request. Record is the replacement value and
    /// ExpectedVersion is the caller-observed version that must still be current.
    /// </summary>
    public class VersionedUpdateRequest<T>
    {
        public T Record { get; set; }
        public string ExpectedVersion { get; set; }
    }
}
