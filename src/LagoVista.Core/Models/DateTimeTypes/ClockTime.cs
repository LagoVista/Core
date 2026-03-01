using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    using System;
    using System.Globalization;

    public struct ClockTime : IEquatable<ClockTime>
    {
        private const string Format = "HH':'mm";

        private readonly string _value;

        public string Value => _value;

        public ClockTime(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length != 5)
                throw new FormatException("ClockTime must be exactly 5 characters (HH:mm).");

            DateTime dt;
            if (!DateTime.TryParseExact(
                    value,
                    Format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt))
            {
                throw new FormatException($"Invalid ClockTime: '{value}'. Expected exactly 5 characters in HH:mm (24-hour clock).");
            }

            _value = dt.ToString(Format, CultureInfo.InvariantCulture);
        }

        public override string ToString() => _value;

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public bool Equals(ClockTime other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is ClockTime other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public static implicit operator string(ClockTime time) => time._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator ClockTime(string value) => new ClockTime(value);
#else
         public static explicit operator ClockTime(string value) => new ClockTime(value);
#endif
    

        public static bool TryCreate(string value, out ClockTime result)
        {
            try
            {
                result = new ClockTime(value);
                return true;
            }
            catch
            {
                result = default(ClockTime);
                return false;
            }
        }
    }
}
