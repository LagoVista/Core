using LagoVista.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LagoVista
{
    /// <summary>
    /// Represents a normalized identifier used primarily for document storage systems.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="NormalizedId32"/> enforces a strict identifier format designed for use in
    /// document databases and distributed systems where identifiers are stored and transported
    /// as strings rather than binary GUID values.
    /// </para>
    ///
    /// <para>
    /// The format requirements are:
    /// <list type="bullet">
    /// <item>Exactly 32 characters</item>
    /// <item>Only uppercase letters <c>A-Z</c></item>
    /// <item>Digits <c>0-9</c></item>
    /// <item>No hyphens or punctuation</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Example valid value:
    /// <code>F47AC10B58CC4372A5670E02B2C3D479</code>
    /// </para>
    ///
    /// <para>
    /// This format corresponds to the canonical GUID "N" format (32 hexadecimal characters)
    /// converted to uppercase. The value can therefore be losslessly converted to and from
    /// a <see cref="Guid"/> or <see cref="GuidString36"/> when it originates from a GUID.
    /// </para>
    ///
    /// <para>
    /// This type exists to:
    /// <list type="bullet">
    /// <item>Provide a strongly-typed identifier for document stores</item>
    /// <item>Eliminate ambiguity between GUID formats and normalized IDs</item>
    /// <item>Enforce strict validation at construction time</item>
    /// </list>
    /// </para>
    /// </remarks>
    [TypeConverter(typeof(NormalizedId32TypeConverter))]
    [JsonConverter(typeof(NormalizedId32JsonConverter))]
    public readonly struct NormalizedId32 : IEquatable<NormalizedId32>
    {
        private readonly string _value;

        /// <summary>
        /// Gets the normalized identifier value.
        /// </summary>
        /// <remarks>
        /// The returned string is guaranteed to be exactly 32 characters and contain
        /// only uppercase letters and digits.
        /// </remarks>
        public string Value => _value;

        /// <summary>
        /// Initializes a new instance of <see cref="NormalizedId32"/>.
        /// </summary>
        /// <param name="value">
        /// A 32-character uppercase identifier consisting only of <c>A-Z</c> and <c>0-9</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when the value does not match the required identifier format.
        /// </exception>
        public NormalizedId32(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!IsNormalizedId32(value))
                throw new FormatException(
                    $"Invalid NormalizedId32: '{value}'. Expected 32 chars of A-Z and 0-9.");

            _value = value;
        }

        /// <summary>
        /// Returns the underlying normalized identifier string.
        /// </summary>
        public override string ToString() => _value;

        /// <inheritdoc/>
        public bool Equals(NormalizedId32 other) =>
            string.Equals(_value, other._value, StringComparison.Ordinal);

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            obj is NormalizedId32 other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            _value?.GetHashCode() ?? 0;

        /// <summary>
        /// Implicitly converts a <see cref="NormalizedId32"/> to its underlying string value.
        /// </summary>
        public static implicit operator string(NormalizedId32 id) => id._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        /// <summary>
        /// Implicitly converts a string to <see cref="NormalizedId32"/>.
        /// Intended for migration scenarios where identifiers are still represented as strings.
        /// </summary>
        public static implicit operator NormalizedId32(string value) => new NormalizedId32(value);
#else
        /// <summary>
        /// Explicitly converts a string to <see cref="NormalizedId32"/>.
        /// </summary>
        public static explicit operator NormalizedId32(string value) => new NormalizedId32(value);
#endif

        /// <summary>
        /// Attempts to create a <see cref="NormalizedId32"/> from a string without throwing.
        /// </summary>
        /// <param name="value">The candidate identifier string.</param>
        /// <param name="result">The resulting identifier if successful.</param>
        /// <returns>
        /// <c>true</c> if the value was valid and a <see cref="NormalizedId32"/> was created;
        /// otherwise <c>false</c>.
        /// </returns>
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

        public static NormalizedId32 Parse(string value)
        {
            return new NormalizedId32(value);
        }

        /// <summary>
        /// Converts this normalized identifier to a <see cref="GuidString36"/>.
        /// </summary>
        /// <remarks>
        /// This operation assumes the identifier originated from a GUID. If the value
        /// contains characters outside the hexadecimal range (<c>0-9</c>, <c>A-F</c>),
        /// parsing will fail and an exception will be thrown.
        /// </remarks>
        /// <returns>A canonical GUID string representation.</returns>
        public GuidString36 ToGuidString36()
        {
            var guid = Guid.ParseExact(_value, "N");
            return new GuidString36(guid.ToString("D").ToLowerInvariant());
        }

        /// <summary>
        /// Creates a new normalized identifier derived from a newly generated GUID.
        /// </summary>
        /// <returns>A new <see cref="NormalizedId32"/> instance.</returns>
        public static NormalizedId32 Factory() => Guid.NewGuid().ToId();

        /// <summary>
        /// Determines whether a string matches the normalized identifier format.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <returns>
        /// <c>true</c> if the value is exactly 32 characters and contains only
        /// uppercase letters and digits; otherwise <c>false</c>.
        /// </returns>
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

    public class NormalizedId32TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw new NotSupportedException("Cannot convert null to NormalizedId32.");

            if (value is string str)
                return new NormalizedId32(str);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is NormalizedId32 id)
                return id.Value;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class NormalizedId32JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(NormalizedId32);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert null value to NormalizedId32.");
            }

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing NormalizedId32. Expected String.");

            var value = reader.Value?.ToString();

            if (String.IsNullOrWhiteSpace(value))
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert empty value to NormalizedId32.");
            }

            return new NormalizedId32(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((NormalizedId32)value).Value);
        }
    }
}