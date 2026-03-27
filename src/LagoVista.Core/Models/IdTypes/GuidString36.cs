using LagoVista.Core.AI.Models;
using LagoVista.Core.Models.ML;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace LagoVista
{
    /// <summary>
    /// Represents a strictly validated GUID string in canonical "D" format.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type enforces a deterministic textual representation of a <see cref="Guid"/> for use
    /// in systems where identifiers must be transmitted or stored as strings.
    /// </para>
    ///
    /// <para>
    /// The format enforced by <see cref="GuidString36"/> is:
    /// <list type="bullet">
    /// <item>Exactly 36 characters</item>
    /// <item>Lowercase hexadecimal digits</item>
    /// <item>Hyphens at positions 8, 13, 18, and 23</item>
    /// <item>No surrounding braces</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Example valid value:
    /// <code>f47ac10b-58cc-4372-a567-0e02b2c3d479</code>
    /// </para>
    ///
    /// <para>
    /// This type intentionally rejects other GUID string formats such as:
    /// <list type="bullet">
    /// <item>Uppercase GUIDs</item>
    /// <item>GUIDs wrapped in braces</item>
    /// <item>32-character normalized identifiers (<see cref="NormalizedId32"/>)</item>
    /// <item>Any non-canonical GUID representation</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Internally, values are always normalized to lowercase canonical "D" format.
    /// </para>
    ///
    /// <para>
    /// This type is commonly used at relational boundaries where GUID values are represented
    /// as strings but must remain strictly validated and convertible to <see cref="Guid"/>.
    /// </para>
    /// </remarks>
    [TypeConverter(typeof(GuidString36TypeConverter))]
    [JsonConverter(typeof(GuidString36JsonConverter))]
    public readonly struct GuidString36 : IEquatable<GuidString36>
    {
        private readonly string _value;

        /// <summary>
        /// Gets the canonical GUID string value in lowercase "D" format.
        /// </summary>
        public string Value => _value;

        /// <summary>
        /// Creates a new <see cref="GuidString36"/> containing a newly generated GUID.
        /// </summary>
        /// <returns>A new canonical GUID string.</returns>
        public static GuidString36 Factory() => new GuidString36(Guid.NewGuid().ToString("D"));

        /// <summary>
        /// Initializes a new instance of <see cref="GuidString36"/> from a string value.
        /// </summary>
        /// <param name="value">
        /// A GUID string in canonical lowercase "D" format
        /// (<c>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</c>).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown if the value does not match the strict GUID format or is not a valid GUID.
        /// </exception>
        public GuidString36(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!IsStrictLowerD(value))
                throw new FormatException(
                    $"Invalid GuidString36: '{value}'. Expected lowercase GUID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.");

            var guid = Guid.ParseExact(value, "D");
            _value = guid.ToString("D").ToLowerInvariant();
        }

        /// <summary>
        /// Direct mapping from Guid
        /// </summary>
        /// <param name="guid"></param>
        public GuidString36(Guid guid)
        {
            _value = guid.ToString("D").ToLowerInvariant();
        }

        /// <summary>
        /// Create from Guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static GuidString36 FromGuid(Guid guid) => new GuidString36(guid);

        /// <summary>
        /// Returns the canonical GUID string representation.
        /// </summary>
        public override string ToString() => _value;

        /// <summary>
        /// Converts this value to a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The equivalent <see cref="Guid"/>.</returns>
        public Guid ToGuid()
        {
            return Guid.ParseExact(_value, "D");
        }

        public Guid DbId => ToGuid();

        /// <inheritdoc/>
        public bool Equals(GuidString36 other) =>
            string.Equals(_value, other._value, StringComparison.Ordinal);

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            obj is GuidString36 other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            _value?.GetHashCode() ?? 0;

        /// <summary>
        /// Implicitly converts a <see cref="GuidString36"/> to its underlying string value.
        /// </summary>
        public static implicit operator string(GuidString36 id) => id._value;

        /// <summary>
        /// Determines whether two <see cref="GuidString36"/> values are equal.
        /// </summary>
        public static bool operator ==(GuidString36 left, GuidString36 right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="GuidString36"/> values are not equal.
        /// </summary>
        public static bool operator !=(GuidString36 left, GuidString36 right) => !left.Equals(right);

        /// <summary>
        /// Compares a <see cref="GuidString36"/> with a nullable <see cref="Guid"/>.
        /// </summary>
        public static bool operator ==(GuidString36 left, Guid? right)
        {
            if (!right.HasValue)
                return false;

            return left.ToGuid() == right.Value;
        }

        /// <summary>
        /// Determines inequality between a <see cref="GuidString36"/> and a nullable <see cref="Guid"/>.
        /// </summary>
        public static bool operator !=(GuidString36 left, Guid? right)
        {
            if (!right.HasValue)
                return true;

            return left.ToGuid() != right.Value;
        }

        /// <summary>
        /// Compares a nullable <see cref="Guid"/> with a <see cref="GuidString36"/>.
        /// </summary>
        public static bool operator ==(Guid? left, GuidString36 right)
        {
            if (!left.HasValue)
                return false;

            return left.Value == right.ToGuid();
        }

        /// <summary>
        /// Determines inequality between a nullable <see cref="Guid"/> and a <see cref="GuidString36"/>.
        /// </summary>
        public static bool operator !=(Guid? left, GuidString36 right)
        {
            if (!left.HasValue)
                return true;

            return left.Value != right.ToGuid();
        }

        /// <summary>
        /// Compares a <see cref="GuidString36"/> with a <see cref="Guid"/>.
        /// </summary>
        public static bool operator ==(GuidString36 left, Guid right) => left.ToGuid() == right;

        /// <summary>
        /// Determines inequality between a <see cref="GuidString36"/> and a <see cref="Guid"/>.
        /// </summary>
        public static bool operator !=(GuidString36 left, Guid right) => left.ToGuid() != right;

        /// <summary>
        /// Compares a <see cref="Guid"/> with a <see cref="GuidString36"/>.
        /// </summary>
        public static bool operator ==(Guid left, GuidString36 right) => left == right.ToGuid();

        /// <summary>
        /// Determines inequality between a <see cref="Guid"/> and a <see cref="GuidString36"/>.
        /// </summary>
        public static bool operator !=(Guid left, GuidString36 right) => left != right.ToGuid();

#if MIGRATION_IMPLICIT_WIRE_TYPES
        /// <summary>
        /// Implicitly converts a string to <see cref="GuidString36"/>.
        /// Intended for migration scenarios only.
        /// </summary>
        public static implicit operator GuidString36(string value) => new GuidString36(value);
#else
        /// <summary>
        /// Explicitly converts a string to <see cref="GuidString36"/>.
        /// </summary>
        public static explicit operator GuidString36(string value) => new GuidString36(value);
#endif

        /// <summary>
        /// Explicitly converts a <see cref="GuidString36"/> to a <see cref="Guid"/>.
        /// </summary>
        public static explicit operator Guid(GuidString36 id) => id.ToGuid();

        /// <summary>
        /// Converts this GUID into a <see cref="NormalizedId32"/> representation.
        /// </summary>
        /// <returns>
        /// A 32-character uppercase identifier suitable for document storage systems.
        /// </returns>
        public NormalizedId32 ToNormalizedId32()
        {
            var guid = Guid.ParseExact(_value, "D");
            var n = guid.ToString("N").ToUpperInvariant();
            return new NormalizedId32(n);
        }

        /// <summary>
        /// Attempts to create a <see cref="GuidString36"/> from a string without throwing.
        /// </summary>
        /// <param name="value">The candidate GUID string.</param>
        /// <param name="result">The parsed value if successful.</param>
        /// <returns><c>true</c> if the value was valid; otherwise <c>false</c>.</returns>
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
                }
            }

            result = default;
            return false;
        }

        public static GuidString36 Parse(string value) => new GuidString36(value);

        /// <summary>
        /// Determines whether a string matches the strict lowercase GUID "D" format.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <returns>
        /// <c>true</c> if the string matches the required format; otherwise <c>false</c>.
        /// </returns>
        public static bool IsStrictLowerD(string value)
        {
            if (value == null || value.Length != 36) return false;

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

    public class GuidString36TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(Guid) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || destinationType == typeof(Guid) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw new NotSupportedException("Cannot convert null to GuidString36.");

            if (value is string str)
                return new GuidString36(str);

            if (value is Guid guid)
                return new GuidString36(guid);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is GuidString36 id)
            {
                if (destinationType == typeof(string))
                    return id.Value;

                if (destinationType == typeof(Guid))
                    return id.ToGuid();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class GuidString36JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(GuidString36);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert null value to GuidString36.");
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (String.IsNullOrWhiteSpace(value))
                {
                    if (isNullable)
                        return null;

                    throw new JsonSerializationException("Cannot convert empty value to GuidString36.");
                }

                return new GuidString36(value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (String.IsNullOrWhiteSpace(value))
                    throw new JsonSerializationException("Cannot convert empty value to GuidString36.");

                return new GuidString36(value);
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing GuidString36. Expected String or Guid.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((GuidString36)value).Value);
        }
    }
}