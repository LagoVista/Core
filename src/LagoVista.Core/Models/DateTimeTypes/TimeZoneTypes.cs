using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.DateTimeTypes
{
    public class TimeZoneReference
    {
        public int IntId { get; set; }
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string StandardName { get; set; }
        public string DaylightName { get; set; }
        public string BaseUtcOffset { get; set; }
        public List<TimeZoneAdjustmentRuleReference> AdjustmentRules { get; set; }
        public bool SupportsDaylightSavingTime { get; set; }
    }

    public class TimeZoneAdjustmentRuleReference
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public TimeSpan DaylightDelta { get; set; }
        public TimeZoneTransitionTimeReference DaylightTransitionStart { get; set; }
        public TimeZoneTransitionTimeReference DaylightTransitionEnd { get; set; }
        public TimeSpan BaseUtcOffsetDelta { get; set; }
        public bool NoDaylightTransitions { get; set; }
    }

    public class TimeZoneTransitionTimeReference
    {
        public DateTime TimeOfDay { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsFixedDateRule { get; set; }
    }
}
