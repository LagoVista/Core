using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ITimeZoneServices
    {
        List<TimeZoneInfo> GetTimeZones();
        TimeZoneInfo GetTimeZoneById(string id);
    }
}
