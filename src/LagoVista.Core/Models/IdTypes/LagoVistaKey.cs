using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace LagoVista
{
    [TypeConverter(typeof(LagoVistaKeyTypeConverter))]
    [JsonConverter(typeof(LagoVistaKeyJsonConverter))]
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
                    "Must be 3–64 chars, lowercase a-z and 0-9 only, and start with a letter.  It may also be a legacy generated key from an id");

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
            return IsStrictBusinessKey(value) || IsLegacyNormalizedGuidLikeKey(value);
        }

        private static bool IsStrictBusinessKey(string value)
        {
            var length = value.Length;
            if (length < 3 || length > 64)
                return false;

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

        private static bool IsLegacyNormalizedGuidLikeKey(string value)
        {
            if (value.Length != 32)
                return false;

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                var isLower = c >= 'a' && c <= 'z';
                var isDigit = c >= '0' && c <= '9';

                if (!isLower && !isDigit)
                    return false;
            }

            return true;
        }

        public static LagoVistaKey Parse(string value)
        {
            return new LagoVistaKey(value);
        }
    }

    public class LagoVistaKeyTypeConverter : TypeConverter
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
                throw new NotSupportedException("Cannot convert null to LagoVistaKey.");

            if (value is string str)
                return new LagoVistaKey(str);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is LagoVistaKey key)
                return key.Value;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class LagoVistaKeyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(LagoVistaKey);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException($"Cannot convert null value to LagoVistaKey at path '{reader.Path}'.");
            }

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing LagoVistaKey at path '{reader.Path}'. Expected String.");

            var value = reader.Value?.ToString();

            if (String.IsNullOrWhiteSpace(value))
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException($"Cannot convert empty value to LagoVistaKey at path '{reader.Path}'.");
            }

            return new LagoVistaKey(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((LagoVistaKey)value).Value);
        }
    }
}