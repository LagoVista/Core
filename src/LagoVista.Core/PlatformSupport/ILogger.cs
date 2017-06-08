using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public enum LogLevel
    {
        UserEntryError,
        Error,
        UnhandledException,
        Warning,
        Message,
        Verbose,
    }

    public interface ILogger
    {
        TimedEvent StartTimedEvent(string area, string description);
        
        void EndTimedEvent(TimedEvent evt);

        void SetUserId(string userId);
        void SetKeys(params KeyValuePair<String, String>[] args);

        void Log(LogLevel level, String area, String message, params KeyValuePair<String, String>[] args);

        void LogException(String area, Exception ex, params KeyValuePair<String, String>[] args);

        void TrackEvent(string message, Dictionary<string, string> parameters);
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
