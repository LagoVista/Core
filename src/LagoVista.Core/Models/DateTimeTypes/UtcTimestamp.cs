using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    using System;
    using System.Globalization;

    public struct UtcTimestamp : IEquatable<UtcTimestamp>
    {
        private static readonly string[] AcceptedFormats = new[]
        {
        "yyyy-MM-dd'T'HH':'mm'Z'",
        "yyyy-MM-dd'T'HH':'mm':'ss'Z'",
        "yyyy-MM-dd'T'HH':'mm':'ss'.'fff'Z'"
    };

        private const string CanonicalFormat = "yyyy-MM-dd'T'HH':'mm':'ss'.'fff'Z'";

        private readonly string _value;

        public string Value => _value;

        public UtcTimestamp(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!value.EndsWith("Z", StringComparison.Ordinal))
                throw new FormatException("UtcTimestamp must end with 'Z'.");

            DateTime dt;
            if (!DateTime.TryParseExact(
                    value,
                    AcceptedFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out dt))
            {
                throw new FormatException(
                    $"Invalid UtcTimestamp: '{value}'. Expected ISO8601 UTC ending with 'Z'. " +
                    $"Accepted patterns: yyyy-MM-ddTHH:mmZ | yyyy-MM-ddTHH:mm:ssZ | yyyy-MM-ddTHH:mm:ss.fffZ.");
            }

            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            _value = dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture);
        }

        public DateTime ToDateTimeUtc()
        {
            var dt = DateTime.ParseExact(
                _value,
                CanonicalFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        public override string ToString() => _value;

        public bool Equals(UtcTimestamp other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is UtcTimestamp other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public static implicit operator string(UtcTimestamp ts) => ts._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator UtcTimestamp(string value) => new UtcTimestamp(value);
#else
        public static explicit operator UtcTimestamp(string value) => new UtcTimestamp(value);
#endif



        public static bool TryCreate(string value, out UtcTimestamp result)
        {
            try
            {
                result = new UtcTimestamp(value);
                return true;
            }
            catch
            {
                result = default(UtcTimestamp);
                return false;
            }
        }
    }
}
