// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c6adf4d2574b12f093a045b22c8a392dd60bd2ef67752b3d3a05e51302225458
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        List<EnumDescription> GetTimeZoneEnumOptions();
}
}
