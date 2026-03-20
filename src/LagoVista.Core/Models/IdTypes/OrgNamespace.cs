using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace LagoVista.Core
{
    /// <summary>
    /// Represents a tenant/organization namespace.
    ///
    /// Rules:
    /// - 6–64 characters
    /// - lowercase letters only (a-z)
    /// </summary>
    [TypeConverter(typeof(OrgNamespaceTypeConverter))]
    [JsonConverter(typeof(OrgNamespaceJsonConverter))]
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
                    $"Invalid OrgNamespace: '{value}'. " +
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
        public static explicit operator OrgNamespace(string value) => new OrgNamespace(value);
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

        public static OrgNamespace Parse(string value)
        {
            return new OrgNamespace(value);
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

    public class OrgNamespaceTypeConverter : TypeConverter
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
                throw new NotSupportedException("Cannot convert null to OrgNamespace.");

            if (value is string str)
                return new OrgNamespace(str);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is OrgNamespace orgNamespace)
                return orgNamespace.Value;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class OrgNamespaceJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(OrgNamespace);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert null value to OrgNamespace.");
            }

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing OrgNamespace. Expected String.");

            var value = reader.Value?.ToString();

            if (String.IsNullOrWhiteSpace(value))
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert empty value to OrgNamespace.");
            }

            return new OrgNamespace(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((OrgNamespace)value).Value);
        }
    }
}