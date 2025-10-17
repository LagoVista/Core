// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 876fd08a7020829db6a030f6c495202f627ac5750b284bc132fa1ebc6cf4530e
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.AsyncCoupler.Utils.Tests
{
    public class TestLogger : ILogger
    {
        public bool DebugMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddCustomEvent(LogLevel level, string tag, string customEvent, params KeyValuePair<string, string>[] args)
        {
            
        }

        public void AddException(string tag, Exception ex, params KeyValuePair<string, string>[] args)
        {
            
        }

        public void AddKVPs(params KeyValuePair<string, string>[] args)
        {
            
        }

        public void EndTimedEvent(TimedEvent evt)
        {
        }

        public TimedEvent StartTimedEvent(string area, string description)
        {
            return new TimedEvent(area, description);
        }

        public void Trace(string message, params KeyValuePair<string, string>[] args)
        {
        
        }

        public void TrackEvent(string message, Dictionary<string, string> parameters)
        {
            
        }

        public void TrackMetric(string kind, string name, MetricType metricType, double count, params KeyValuePair<string, string>[] args)
        {
        }

        public void TrackMetric(string kind, string name, MetricType metricType, int count, params KeyValuePair<string, string>[] args)
        {
        }
    }

    //public class LogWriter : ILogWriter
    //{
    //    public List<LogRecord> ErrorRecords = new List<LogRecord>();

    //    public Task WriteError(LogRecord record)
    //    {
    //        ErrorRecords.Add(record);

    //        Console.WriteLine(record.Tag);
    //        Console.WriteLine(record.Message);
    //        if (!String.IsNullOrEmpty(record.Details))
    //            Console.WriteLine(record.Details);
    //        return Task.FromResult(default(object));
    //    }

    //    public Task WriteEvent(LogRecord record)
    //    {
    //        Console.WriteLine(record.Tag);
    //        Console.WriteLine(record.Message);
    //        if (!String.IsNullOrEmpty(record.Details))
    //            Console.WriteLine(record.Details);
    //        return Task.FromResult(default(object));
    //    }
    //}
}
