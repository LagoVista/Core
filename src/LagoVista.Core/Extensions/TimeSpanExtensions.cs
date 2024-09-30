using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista
{
    public static class TimeSpanHelpers
    {
        public static string ToDescription(this TimeSpan timeSpanOverdue)
        {
            if (timeSpanOverdue.Days > 0)
                return $"{timeSpanOverdue.Days}day(s), {timeSpanOverdue.Hours}hr(s) {timeSpanOverdue.Minutes}min(s)";
            else if (timeSpanOverdue.Hours > 0)
                return $"{timeSpanOverdue.Hours}hr(s) {timeSpanOverdue.Minutes}min(s)";
            else if (timeSpanOverdue.Minutes > 0)
                return $"{timeSpanOverdue.Minutes}min(s) {timeSpanOverdue.Seconds}second(s)";
            else
                return $"{timeSpanOverdue.Seconds}second(s)";
        }
    }
}
