// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 092df060639b43208cbc0cf405c2bf12790069a6ddb580954580e6b1d494399b
// IndexVersion: 2
// --- END CODE INDEX META ---
namespace LagoVista.Core.Utils.Types
{
    public sealed class RawInput
    {
        public string MimeType { get; set; }      // if null, defaults to "text/plain"
        public string RawText { get; set; }       // preferred when source is text (markdown/html/plain)
                                                  // If you later add streaming/bytes, extend this class (kept minimal per your request)
    }
}