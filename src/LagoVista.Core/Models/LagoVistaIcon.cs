
using System;

namespace LagoVista
{
    /// <summary>
    /// Represents an icon identifier used in LagoVista entities.
    ///
    /// Currently behaves as a strict non-empty string.
    /// Future versions may support richer icon metadata.
    /// </summary>
    public readonly struct LagoVistaIcon : IEquatable<LagoVistaIcon>
    {
        private readonly string _value;

        public string Value => _value;

        public LagoVistaIcon(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrWhiteSpace(value))
                throw new FormatException("LagoVistaIcon cannot be empty or whitespace.");

            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(LagoVistaIcon other)
            => string.Equals(_value, other._value, StringComparison.Ordinal);

        public override bool Equals(object obj)
            => obj is LagoVistaIcon other && Equals(other);

        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        public static implicit operator string(LagoVistaIcon icon) => icon._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator LagoVistaIcon(string value) => new LagoVistaIcon(value);
#else
        public static explicit operator LagoVistaIcon(string value) => new LagoVistaIcon(value);
#endif

        public static bool TryCreate(string value, out LagoVistaIcon result)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                result = new LagoVistaIcon(value);
                return true;
            }

            result = default;
            return false;
        }
    }
}
