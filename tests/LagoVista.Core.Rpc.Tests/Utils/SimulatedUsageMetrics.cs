// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2a22b815431283423260a1dcabd9e5d756efab2c2ffddcec386b8e0025f32a9a
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using System;

namespace LagoVista.Core.Rpc.Tests.Utils
{
    public sealed class SimulatedUsageMetrics : IUsageMetrics
    {
        public SimulatedUsageMetrics(String hostId, String instanceId, string pipelineModuleId)
        {
            HostId = HostId;
            InstanceId = instanceId;
            PipelineModuleId = pipelineModuleId;
        }

        private SimulatedUsageMetrics() { }

        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public long BytesProcessed { get; set; }
        public int DeadLetterCount { get; set; }
        public int MessagesProcessed { get; set; }
        public string PipelineModuleId { get; set; }
        public string Status { get; set; }
        public string HostId { get; set; }
        public string InstanceId { get; set; }
        public string Version { get; set; }
        public double AverageProcessingMS { get; set; }
        public double MessagesPerSecond { get; set; }
        public double ElapsedMS { get; set; }
        public string EndTimeStamp { get; set; }
        public string StartTimeStamp { get; set; }
        public int ActiveCount { get; set; }
        public double ProcessingMS { get; set; }

        public void Calculate()
        {
            ElapsedMS = Math.Round((EndTimeStamp.ToDateTime() - StartTimeStamp.ToDateTime()).TotalMilliseconds, 3);

            if (ElapsedMS > 1)
            {
                MessagesPerSecond = (MessagesProcessed / ElapsedMS) * 1000.0;
            }

            if (MessagesProcessed > 0)
            {
                AverageProcessingMS = Math.Round(ProcessingMS / MessagesProcessed, 3);
            }
        }

        public IUsageMetrics Clone()
        {
            var clonedMetric = new SimulatedUsageMetrics()
            {
                Version = Version,
                HostId = HostId,
                InstanceId = InstanceId,
                PipelineModuleId = PipelineModuleId,

                ErrorCount = ErrorCount,
                BytesProcessed = BytesProcessed,
                WarningCount = WarningCount,
                ProcessingMS = ProcessingMS,
                DeadLetterCount = DeadLetterCount,
                ActiveCount = ActiveCount,
                MessagesProcessed = MessagesProcessed,
            };

            return clonedMetric;
        }

        public void Concat(IUsageMetrics metric)
        {
            ErrorCount += metric.ErrorCount;
            BytesProcessed += metric.BytesProcessed;
            WarningCount += metric.WarningCount;
            ProcessingMS += metric.ProcessingMS;
            DeadLetterCount += metric.DeadLetterCount;
            ActiveCount += metric.ActiveCount;
        }

        public void Reset(string previousEndTime = null)
        {
            StartTimeStamp = previousEndTime ?? DateTime.UtcNow.ToJSONString();
            EndTimeStamp = String.Empty;

            ElapsedMS = 0.0;

            MessagesProcessed = 0;
            BytesProcessed = 0;
            ErrorCount = 0;
            WarningCount = 0;
            ProcessingMS = 0;
            DeadLetterCount = 0;
        }

        public void SetDatestamp(DateTime dateStamp)
        {
            
        }
    }
}
