using System;

namespace LagoVista.Core
{
    /// <summary>
    /// Represents a tenant/organization namespace.
    ///
    /// Rules:
    /// - 6–64 characters
    /// - lowercase letters only (a-z)
    /// </summary>
    public readonly struct OrgNamespace : IEquatable<OrgNamespace>
    {
        private readonly string _value;

        public string Value => _value;

        public OrgNamespace(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!IsValid(value))
                throw new FormatException(
                    $"Invalid TenantNamespace: '{value}'. " +
                    "Must be 6–64 lowercase letters (a-z) only.");

            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(OrgNamespace other)
            => string.Equals(_value, other._value, StringComparison.Ordinal);

        public override bool Equals(object obj)
            => obj is OrgNamespace other && Equals(other);

        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        public static implicit operator string(OrgNamespace ns) => ns._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator OrgNamespace(string value) => new OrgNamespace(value);
#else
        public static explicit operator TenantNamespace(string value) => new TenantNamespace(value);
#endif

        public static bool TryCreate(string value, out OrgNamespace result)
        {
            if (value != null && IsValid(value))
            {
                result = new OrgNamespace(value);
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsValid(string value)
        {
            if (value == null) return false;

            var length = value.Length;
            if (length < 6 || length > 64)
                return false;

            for (var i = 0; i < length; i++)
            {
                var c = value[i];
                if (c < 'a' || c > 'z')
                    return false;
            }

            return true;
        }
    }
}