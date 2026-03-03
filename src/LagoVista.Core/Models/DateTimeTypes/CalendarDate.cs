using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    using System;
    using System.Globalization;

    public struct CalendarDate : IEquatable<CalendarDate>
    {
        private static readonly string[] AcceptedFormats = new[]
        {
        "yyyy/MM/dd",
        "yyyy-MM-dd"
    };

        private const string CanonicalFormat = "yyyy-MM-dd";

        private readonly string _value;

        public string Value => _value;

        public CalendarDate(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length != 10)
                throw new FormatException($"Invalid CalendarDate: '{value}'. Expected exactly 10 characters in yyyy/MM/dd or yyyy-MM-dd.");

            DateTime dt;
            if (!DateTime.TryParseExact(
                    value,
                    AcceptedFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt))
            {
                throw new FormatException(
                    $"Invalid CalendarDate: '{value}'. Expected exactly 10 characters in yyyy/MM/dd or yyyy-MM-dd.");
            }

            _value = dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture);
        }

        public DateTime ToDateTime()
        {
            return DateTime.ParseExact(
                _value,
                CanonicalFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None).Date;
        }

        public int Year => ToDateTime().Year;
        public int Month => ToDateTime().Month;
        public int Day => ToDateTime().Day;

        public static CalendarDate Today()
        {
            return new CalendarDate(DateTime.Today.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate StartOfMonth(int year, int month)
        {
            // DateTime ctor validates year/month (throws if invalid)
            var dt = new DateTime(year, month, 1);
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public CalendarDate AddDays(int days)
        {
            var dt = ToDateTime().AddDays(days);
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate EndOfMonth(int year, int month)
        {
            // DateTime ctor validates year/month (throws if invalid)
            var dt = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate Create(int year, int month, int day)
        {
            // DateTime ctor validates year/month (throws if invalid)
            var dt = new DateTime(year, month, day);
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public override string ToString() => _value;

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public bool Equals(CalendarDate other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is CalendarDate other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public static implicit operator string(CalendarDate date) => date._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator CalendarDate(string value) => new CalendarDate(value);
#else
        public static explicit operator CalendarDate(string value) => new CalendarDate(value);

#endif
        public static bool TryCreate(string value, out CalendarDate result)
        {
            try
            {
                result = new CalendarDate(value);
                return true;
            }
            catch
            {
                result = default(CalendarDate);
                return false;
            }
        }
    }
}
