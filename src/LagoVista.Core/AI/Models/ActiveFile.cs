namespace LagoVista.Core.AI.Models
{
    public class ActiveFile
    {
        public string AbsolutePath { get; set; }
        public string RelativePath { get; set; }
        public string FileName { get; set; }
        public string Contents { get; set; }
        public string Language { get; set; }
        public string MimeType { get; set; }
    }
}
