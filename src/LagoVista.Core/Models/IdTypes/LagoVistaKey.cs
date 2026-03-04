using System;

namespace LagoVista
{
    /// <summary>
    /// Represents a strict LagoVista document key.
    ///
    /// Rules:
    /// - 3–64 characters
    /// - lowercase letters (a-z) and digits (0-9) only
    /// - must start with a lowercase letter
    /// </summary>
    public readonly struct LagoVistaKey : IEquatable<LagoVistaKey>
    {
        private readonly string _value;

        public string Value => _value;

        public LagoVistaKey(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!IsValid(value))
                throw new FormatException(
                    $"Invalid LagoVistaKey: '{value}'. " +
                    "Must be 3–64 chars, lowercase a-z and 0-9 only, and start with a letter.");

            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(LagoVistaKey other)
            => string.Equals(_value, other._value, StringComparison.Ordinal);

        public override bool Equals(object obj)
            => obj is LagoVistaKey other && Equals(other);

        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        public static implicit operator string(LagoVistaKey key) => key._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator LagoVistaKey(string value) => new LagoVistaKey(value);
#else
        public static explicit operator LagoVistaKey(string value) => new LagoVistaKey(value);
#endif

        public static bool TryCreate(string value, out LagoVistaKey result)
        {
            if (value != null && IsValid(value))
            {
                result = new LagoVistaKey(value);
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsValid(string value)
        {
            if (value == null) return false;

            var length = value.Length;
            if (length < 3 || length > 64)
                return false;

            // First character must be lowercase letter
            var first = value[0];
            if (first < 'a' || first > 'z')
                return false;

            for (var i = 1; i < length; i++)
            {
                var c = value[i];

                var isLower = c >= 'a' && c <= 'z';
                var isDigit = c >= '0' && c <= '9';

                if (!isLower && !isDigit)
                    return false;
            }

            return true;
        }
    }
}