namespace LagoVista.Core.Utils.Types
{
    public sealed class RawInput
    {
        public string MimeType { get; set; }      // if null, defaults to "text/plain"
        public string RawText { get; set; }       // preferred when source is text (markdown/html/plain)
                                                  // If you later add streaming/bytes, extend this class (kept minimal per your request)
    }
}