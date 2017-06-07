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
        void SetUserId(string userId);
        void SetKeys(params KeyValuePair<String, String>[] args);

        void Log(LogLevel level, String area, String message, params KeyValuePair<String, String>[] args);

        void LogException(String area, Exception ex, params KeyValuePair<String, String>[] args);

        void TrackEvent(string message, Dictionary<string, string> parameters);
    }
}
