// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 098929fb02566df825332b243a97008c800beb8cc628f63689180c9e966d33cd
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface ITimer : IDisposable
    {
        event EventHandler Tick;
        bool InvokeOnUIThread { get; set; }

        TimeSpan Interval { get; set; }
        Object State { get; set; }

        void Start();
        void Stop();
    }

    public interface ITimerFactory
    {
        ITimer Create(TimeSpan interval);
    }
}
