// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c74317b38186118a775edcbfbda4fb2a774460778c4a732593eaa1421a057fab
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Interfaces
{
    public interface IUsageMetrics
    {
        [global::Newtonsoft.Json.JsonPropertyAttribute("warningCount")]
        int WarningCount { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("errorCount")]
        int ErrorCount { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("bytesProccessed")]
        long BytesProcessed { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("deadLetterCount")]
        int DeadLetterCount { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("messagesProcessed")]
        int MessagesProcessed { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("pipelineModuleId")]
        string PipelineModuleId { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("status")]
        string Status { get; set; }
        
        [global::Newtonsoft.Json.JsonPropertyAttribute("hostId")]
        string HostId { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("instanceId")]
        string InstanceId { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("version")]
        string Version { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("averageProcessingMS")]
        double AverageProcessingMS { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("messagesPerSecond")]
        double MessagesPerSecond { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("elapsedMS")]
        double ElapsedMS { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("endTimeStamp")]
        string EndTimeStamp { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("startTimeStamp")]
        string StartTimeStamp { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("activeCount")]
        int ActiveCount { get; set; }

        [global::Newtonsoft.Json.JsonPropertyAttribute("processingMS")]
        double ProcessingMS { get; set; }

        void Calculate();

        IUsageMetrics Clone();

        void SetDatestamp(DateTime dateStamp);

        void Concat(IUsageMetrics metric);

        void Reset(string previousEndTime = null);
    }
}
