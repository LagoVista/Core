using LagoVista.Core;
using System;
using System.Runtime.CompilerServices;

namespace LagoVista
{
    /// <summary>
    /// Normalized document-storage identifier.
    /// Policy: exactly 32 chars, only A-Z and 0-9.
    /// </summary>
    public readonly struct NormalizedId32 : IEquatable<NormalizedId32>
    {
        private readonly string _value;
        public string Value => _value;

        public NormalizedId32(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!IsNormalizedId32(value))
                throw new FormatException($"Invalid NormalizedId32: '{value}'. Expected 32 chars of A-Z and 0-9.");

            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(NormalizedId32 other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is NormalizedId32 other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public static implicit operator string(NormalizedId32 id) => id._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator NormalizedId32(string value) => new NormalizedId32(value);
#else
        public static explicit operator NormalizedId32(string value) => new NormalizedId32(value);
#endif

        public static bool TryCreate(string value, out NormalizedId32 result)
        {
            if (value != null && IsNormalizedId32(value))
            {
                result = new NormalizedId32(value);
                return true;
            }

            result = default;
            return false;
        }
        public GuidString36 ToGuidString36()
        {
            // We know this is 32 uppercase A-Z0-9.
            // But GUID hex must be 0-9A-F only.
            // If your normalized IDs are guaranteed to originate from GUIDs,
            // this will always succeed. Otherwise it should blow up loudly.

            var guid = Guid.ParseExact(_value, "N");
            return new GuidString36(guid.ToString("D").ToLowerInvariant());
        }

        public static NormalizedId32 Factory() => Guid.NewGuid().ToId();

        public static bool IsNormalizedId32(string value)
        {
            if (value == null || value.Length != 32) return false;

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                var isDigit = c >= '0' && c <= '9';
                var isUpper = c >= 'A' && c <= 'Z';
                if (!isDigit && !isUpper) return false;
            }

            return true;
        }
    }
}