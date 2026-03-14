using LagoVista.Core.Models.DateTimeTypes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ITimeZoneServices
    {
        List<TimeZoneInfo> GetTimeZones();
        TimeZoneInfo GetTimeZoneById(string id);
        TimeZoneInfo GetTimeZoneByIntId(int intId);
        List<EnumDescription> GetTimeZoneEnumOptions();
        TimeZoneReference GetTimeZoneReferenceByIntId(int intId);
    }
}
