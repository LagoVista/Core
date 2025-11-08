// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 98e216496057353e2eed52815a6b0fbdff74568f0b720ca311f47f32730e9148
// IndexVersion: 2
// --- END CODE INDEX META ---
namespace LagoVista.Core.Models
{
    public class JobOutput
    {
        public string JobId { get; set; }
        public bool Success { get; set; }
        public string JobTypeId { get; set; }
        public string JobName { get; set; }
        public double ExecutionTimeSeconds { get; set; }
        public string Notes { get; set; }
        public string Error { get; set; }
        public string ArtifactType { get; set; }
        public string ArtifactId { get; set; }
        public string ArtifactUrl { get; set; }
    }
}
