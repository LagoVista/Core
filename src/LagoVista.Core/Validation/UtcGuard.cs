using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Validation
{
    public static class UtcGuard
    {
        public static DateTime RequireUtc(this DateTime value, string paramName = null)
        {
            if (value.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTime must be UTC (DateTimeKind.Utc).", paramName ?? nameof(value));

            return value;
        }

        public static DateTime? RequireUtc(this DateTime? value, string paramName = null)
        {
            if (value.HasValue && value.Value.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTime must be UTC (DateTimeKind.Utc).", paramName ?? nameof(value));

            return value;
        }
    }
}
