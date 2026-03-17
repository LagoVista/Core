using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IClock
    {
        UtcTimestamp Now { get; }
        CalendarDate Today { get; }
    }

    public class LagoVistaClock : IClock
    {
        public CalendarDate Today { get { return CalendarDate.Today(); } }
        public UtcTimestamp Now { get { return UtcTimestamp.Now; } }
    }
}
