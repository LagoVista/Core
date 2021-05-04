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
