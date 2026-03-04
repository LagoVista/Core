using System;

namespace LagoVista
{
    /// <summary>
    /// Strict GUID string wrapper.
    /// Policy:
    /// - Exactly 36 chars
    /// - Lowercase hex
    /// - Hyphens at 8,13,18,23
    /// - No braces
    ///
    /// This will never accept your NormalizedId32 (which is 32 chars A-Z0-9).
    /// </summary>
    public readonly struct GuidString36 : IEquatable<GuidString36>
    {
        private readonly string _value;
        public string Value => _value;

        public static GuidString36 Factory() => Guid.NewGuid().ToString();

        public GuidString36(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!IsStrictLowerD(value))
                throw new FormatException($"Invalid GuidString36: '{value}'. Expected lowercase GUID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.");

            // ParseExact enforces it's truly a GUID, not just shaped like one
            var guid = Guid.ParseExact(value, "D");

            // Canonicalize to strict lowercase D (in case caller used lowercase already, this is stable)
            _value = guid.ToString("D"); // .NET emits lowercase for "D" by default? not guaranteed; we'll normalize:
            _value = _value.ToLowerInvariant();
        }

        public override string ToString() => _value;

        public Guid ToGuid()
        {
            // Stored canonically as lowercase "D"
            return Guid.ParseExact(_value, "D");
        }

        public bool Equals(GuidString36 other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is GuidString36 other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public static implicit operator string(GuidString36 id) => id._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator GuidString36(string value) => new GuidString36(value);
#else
        public static explicit operator GuidString36(string value) => new GuidString36(value);
#endif

        public static explicit operator Guid(GuidString36 id) => id.ToGuid();

        public NormalizedId32 ToNormalizedId32()
        {
            var guid = Guid.ParseExact(_value, "D");
            var n = guid.ToString("N").ToUpperInvariant();
            return new NormalizedId32(n);
        }

        public static bool TryCreate(string value, out GuidString36 result)
        {
            if (value != null && IsStrictLowerD(value))
            {
                try
                {
                    result = new GuidString36(value);
                    return true;
                }
                catch
                {
                    // Shape matched but not a real GUID (extremely unlikely, but possible if someone sneaks non-hex)
                }
            }

            result = default;
            return false;
        }

        public static bool IsStrictLowerD(string value)
        {
            if (value == null || value.Length != 36) return false;

            // Hyphen positions
            if (value[8] != '-' || value[13] != '-' || value[18] != '-' || value[23] != '-') return false;

            for (var i = 0; i < value.Length; i++)
            {
                if (i == 8 || i == 13 || i == 18 || i == 23) continue;

                var c = value[i];
                var isDigit = c >= '0' && c <= '9';
                var isLowerHex = c >= 'a' && c <= 'f';
                if (!isDigit && !isLowerHex) return false;
            }

            return true;
        }
    }
}