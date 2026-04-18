using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace LagoVista
{
    /// <summary>
    /// Represents an icon identifier used in LagoVista entities.
    ///
    /// Currently behaves as a strict non-empty string.
    /// Future versions may support richer icon metadata.
    /// </summary>
    [TypeConverter(typeof(LagoVistaIconTypeConverter))]
    [JsonConverter(typeof(LagoVistaIconJsonConverter))]
    public readonly struct LagoVistaIcon : IEquatable<LagoVistaIcon>
    {
        private readonly string _value;

        public string Value => _value;

        public LagoVistaIcon(string value)
        {
            if (value == null)
                value = "icon-fo-gears-2";

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

        public static LagoVistaIcon Parse(string value)
        {
            return new LagoVistaIcon(value);
        }
    }

    public class LagoVistaIconTypeConverter : TypeConverter
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
                value = "icon-fo-gears-2";

            if (value is string str)
                return new LagoVistaIcon(str);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is LagoVistaIcon icon)
                return icon.Value;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class LagoVistaIconJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(LagoVistaIcon);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                return new LagoVistaIcon("icon-fo-gears-2");
            }

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing LagoVistaIcon. Expected String.");

            var value = reader.Value?.ToString();

            if (String.IsNullOrWhiteSpace(value))
            {
                if (isNullable)
                    return null;

                return new LagoVistaIcon("icon-fo-gears-2");
            }

            return new LagoVistaIcon(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((LagoVistaIcon)value).Value);
        }
    }
}