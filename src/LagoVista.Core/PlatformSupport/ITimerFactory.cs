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
