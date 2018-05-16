using System;

namespace LagoVista.Core.PlatformSupport
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
