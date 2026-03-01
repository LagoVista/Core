using System;

namespace LagoVista.Core
{
    public static class PeriodExtensions
    {
        /// <summary>
        /// Inclusive/inclusive calendar period membership.
        /// </summary>
        public static bool InPeriod(this CalendarDate value, CalendarDate start, CalendarDate end)
        {
            var d = value.ToDateTime().Date;
            var s = start.ToDateTime().Date;
            var e = end.ToDateTime().Date;

            if (e < s) throw new ArgumentException("End must be >= Start.");

            return d >= s && d <= e;
        }

        /// <summary>
        /// Inclusive business period for a UTC timestamp, achieved via exclusive upper bound on the next day.
        /// Equivalent to: ts >= Start@00:00Z && ts < (End+1)@00:00Z.
        /// </summary>
        public static bool InPeriod(this UtcTimestamp value, CalendarDate start, CalendarDate end)
        {
            var ts = value.ToDateTimeUtc(); // should be Kind.Utc

            var sDate = start.ToDateTime().Date;
            var eDate = end.ToDateTime().Date;

            if (eDate < sDate) throw new ArgumentException("End must be >= Start.");

            var sUtc = DateTime.SpecifyKind(sDate, DateTimeKind.Utc);
            var eExclusiveUtc = DateTime.SpecifyKind(eDate.AddDays(1), DateTimeKind.Utc);

            return ts >= sUtc && ts < eExclusiveUtc;
        }
    }
}