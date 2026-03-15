using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport
{
    public class TestAdminLogger : ILogger
    {
        public List<string> Traces { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
        public bool DebugMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddCustomEvent(object evt) { }

        public void AddCustomEvent(LogLevel level, string tag, string customEvent, params KeyValuePair<string, string>[] args)
        {
            throw new NotImplementedException();
        }

        public void AddException(string source, Exception ex, params KeyValuePair<string, string>[] details)
        {
            Errors.Add($"{source}: {ex.GetType().Name}: {ex.Message}");
        }

        public void AddInfo(string message, params KeyValuePair<string, string>[] details) { }

        public void AddKVPs(params KeyValuePair<string, string>[] args)
        {
            throw new NotImplementedException();
        }

        public void AddWarning(string message, params KeyValuePair<string, string>[] details)
        {
            Errors.Add(message);
        }

        public void Debug(string message, params KeyValuePair<string, string>[] details) { }

        public void EndTimedEvent(TimedEvent evt)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, params KeyValuePair<string, string>[] details)
        {
            Errors.Add(message);
        }

        public void Exception(string message, Exception ex, params KeyValuePair<string, string>[] details)
        {
            Errors.Add($"{message}: {ex.GetType().Name}: {ex.Message}");
        }

        public void Info(string message, params KeyValuePair<string, string>[] details) { }

        public void LogTrace(string message)
        {
            Traces.Add(message);
        }

        public TimedEvent StartTimedEvent(string area, string description)
        {
            throw new NotImplementedException();
        }

        public void Trace(string message, params KeyValuePair<string, string>[] details)
        {
            Traces.Add(message);
        }

        public void TrackEvent(string message, Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public void TrackMetric(string kind, string name, MetricType metricType, double count, params KeyValuePair<string, string>[] args)
        {
            throw new NotImplementedException();
        }

        public void TrackMetric(string kind, string name, MetricType metricType, int count, params KeyValuePair<string, string>[] args)
        {
            throw new NotImplementedException();
        }

        public void Warning(string message, params KeyValuePair<string, string>[] details)
        {
            Errors.Add(message);
        }
    }
}
