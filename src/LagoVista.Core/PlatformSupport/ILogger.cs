using System;
using System.Collections.Generic;

namespace LagoVista.Core.PlatformSupport
{
    public enum LogLevel
    {
        UserEntryError,
        Error,
        UnhandledException,
        Warning,
        Message,
        StateChange,
        Verbose,
        TimedEvent,
        Metric,
        ConfigurationError
    }

    public enum MetricType
    {
        Event,
        Aggregate
    }

    public interface ILogger
    {
        bool DebugMode { get; set; }

        TimedEvent StartTimedEvent(string area, string description);
        void EndTimedEvent(TimedEvent evt);

        void AddKVPs(params KeyValuePair<String, String>[] args);

        void AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel level, string tag, string customEvent, params KeyValuePair<string, string>[] args);

        void AddException(string tag, Exception ex, params KeyValuePair<string, string>[] args);

        void TrackEvent(string message, Dictionary<string, string> parameters);

        void TrackMetric(string kind, string name, MetricType metricType, double count, params KeyValuePair<string, string>[] args);

        void TrackMetric(string kind, string name, MetricType metricType, int count, params KeyValuePair<string, string>[] args);

        void Trace(string message);
    }

    public class TimedEvent
    {
        public TimedEvent(string area, string description)
        {
            StartTime = DateTime.Now;
            Area = area;
            Description = description;
        }

        public DateTime StartTime { get; private set; }

        public string Area { get; private set; }

        public string Description { get; private set; }
    }
}
