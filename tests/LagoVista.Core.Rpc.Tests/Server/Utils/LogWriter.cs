// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6eeb903dee6f901d384b2082c940e0c2df4407a8c946100f65d5937c17740267
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Rpc.Tests.Server.Utils
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
