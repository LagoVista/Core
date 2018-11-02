using System;

namespace LagoVista.Core
{
    public interface IConsoleWriter
    {
        void WriteLine(string message);
        void WriteError(string message);
        void WriteWarning(string message);
    }

    public interface ILocalTime
    {
        DateTime Now { get; }
    }
}
