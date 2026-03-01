using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public readonly struct DatePeriod
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; } // inclusive, date-only semantics (midnight is irrelevant)

        public DatePeriod(DateTime startDate, DateTime endDate)
        {
            // These should be dates at midnight, but we won't assume; we only use Date component.
            var s = startDate.Date;
            var e = endDate.Date;

            if (e < s) throw new ArgumentException("EndDate must be >= StartDate.");

            StartDate = s;
            EndDate = e;
        }

        public DateTime EndExclusiveDate => EndDate.AddDays(1); // date-only boundary for half-open conversion
    }
}
