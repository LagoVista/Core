using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace LagoVista
{
    /// <summary>
    /// Bridge ID that normalizes to GuidString36 (relational).
    /// Accepts: GuidString36, NormalizedId32, or string.
    /// </summary>
    [TypeConverter(typeof(RelationalIdTypeConverter))]
    [JsonConverter(typeof(RelationalIdJsonConverter))]
    public readonly struct RelationalId : IEquatable<RelationalId>
    {
        private readonly GuidString36 _value;
        public GuidString36 Value => _value;

        public RelationalId(GuidString36 value)
        {
            _value = value;
        }

        public RelationalId(NormalizedId32 value)
        {
            _value = value.ToGuidString36();
        }

        public RelationalId(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (GuidString36.IsStrictLowerD(value))
            {
                _value = new GuidString36(value);
                return;
            }

            if (NormalizedId32.IsNormalizedId32(value))
            {
                _value = new NormalizedId32(value).ToGuidString36();
                return;
            }

            throw new FormatException(
                $"Invalid RelationalId: '{value}'. Expected either GuidString36 (lowercase dashed GUID) " +
                "or NormalizedId32 (32 chars A-Z0-9).");
        }

        /// <summary>
        /// Compares a <see cref="RelationalId"/> with a <see cref="Guid"/>.
        /// </summary>
        public static bool operator ==(RelationalId left, Guid right) => left.Value.ToGuid() == right;

        /// <summary>
        /// Determines inequality between a <see cref="RelationalId"/> and a <see cref="Guid"/>.
        /// </summary>
        public static bool operator !=(RelationalId left, Guid right) => left.Value.ToGuid() != right;

        /// <summary>
        /// Compares a <see cref="Guid"/> with a <see cref="RelationalId"/>.
        /// </summary>
        public static bool operator ==(Guid left, RelationalId right) => left == right.Value.ToGuid();

        /// <summary>
        /// Determines inequality between a <see cref="Guid"/> and a <see cref="RelationalId"/>.
        /// </summary>
        public static bool operator !=(Guid left, RelationalId right) => left != right.Value.ToGuid();

        public Guid ToGuid() => _value.ToGuid();

        public Guid DbId => _value.ToGuid();

        public override string ToString() => _value.ToString();

        public bool Equals(RelationalId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is RelationalId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();

        public static implicit operator GuidString36(RelationalId id) => id._value;

        public static implicit operator RelationalId(string value) => new RelationalId(value);
        public static implicit operator RelationalId(GuidString36 value) => new RelationalId(value);
        public static implicit operator RelationalId(NormalizedId32 value) => new RelationalId(value);

        public static RelationalId Parse(string value) => new RelationalId(value);

        public static bool TryCreate(string value, out RelationalId result)
        {
            try
            {
                result = new RelationalId(value);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }

    public class RelationalIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ||
                   sourceType == typeof(GuidString36) ||
                   sourceType == typeof(NormalizedId32) ||
                   sourceType == typeof(Guid) ||
                   base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) ||
                   destinationType == typeof(GuidString36) ||
                   destinationType == typeof(Guid) ||
                   base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw new NotSupportedException("Cannot convert null to RelationalId.");

            if (value is string str)
                return new RelationalId(str);

            if (value is GuidString36 guidString36)
                return new RelationalId(guidString36);

            if (value is NormalizedId32 normalizedId32)
                return new RelationalId(normalizedId32);

            if (value is Guid guid)
                return new RelationalId(new GuidString36(guid));

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is RelationalId relationalId)
            {
                if (destinationType == typeof(string))
                    return relationalId.ToString();

                if (destinationType == typeof(GuidString36))
                    return relationalId.Value;

                if (destinationType == typeof(Guid))
                    return relationalId.ToGuid();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class RelationalIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(RelationalId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert null value to RelationalId.");
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (String.IsNullOrWhiteSpace(value))
                {
                    if (isNullable)
                        return null;

                    throw new JsonSerializationException("Cannot convert empty value to RelationalId.");
                }

                return new RelationalId(value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (String.IsNullOrWhiteSpace(value))
                    throw new JsonSerializationException("Cannot convert empty value to GuidString36.");

                return new RelationalId(new GuidString36(value));
            }


            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing RelationalId. Expected String or Guid.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((RelationalId)value).ToString());
        }
    }

    /// <summary>
    /// Bridge ID that normalizes to NormalizedId32 (document).
    /// Accepts: NormalizedId32, GuidString36, or string.
    /// </summary>
    [TypeConverter(typeof(DocumentIdTypeConverter))]
    [JsonConverter(typeof(DocumentIdJsonConverter))]
    public readonly struct DocumentId : IEquatable<DocumentId>
    {
        private readonly NormalizedId32 _value;
        public NormalizedId32 Value => _value;

        public DocumentId(NormalizedId32 value)
        {
            _value = value;
        }

        public DocumentId(GuidString36 value)
        {
            _value = value.ToNormalizedId32();
        }

        public DocumentId(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (NormalizedId32.IsNormalizedId32(value))
            {
                _value = new NormalizedId32(value);
                return;
            }

            if (GuidString36.IsStrictLowerD(value))
            {
                _value = new GuidString36(value).ToNormalizedId32();
                return;
            }

            throw new FormatException(
                $"Invalid DocumentId: '{value}'. Expected either NormalizedId32 (32 chars A-Z0-9) " +
                "or GuidString36 (lowercase dashed GUID).");
        }

        public override string ToString() => _value.ToString();

        public bool Equals(DocumentId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DocumentId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();

        public static implicit operator NormalizedId32(DocumentId id) => id._value;

        public static implicit operator DocumentId(string value) => new DocumentId(value);
        public static implicit operator DocumentId(NormalizedId32 value) => new DocumentId(value);
        public static implicit operator DocumentId(GuidString36 value) => new DocumentId(value);

        public static DocumentId Parse(string value) => new DocumentId(value);

        public static bool TryCreate(string value, out DocumentId result)
        {
            try
            {
                result = new DocumentId(value);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }

    public class DocumentIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ||
                   sourceType == typeof(GuidString36) ||
                   sourceType == typeof(NormalizedId32) ||
                   sourceType == typeof(Guid) ||
                   base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) ||
                   destinationType == typeof(NormalizedId32) ||
                   destinationType == typeof(GuidString36) ||
                   base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw new NotSupportedException("Cannot convert null to DocumentId.");

            if (value is string str)
                return new DocumentId(str);

            if (value is NormalizedId32 normalizedId32)
                return new DocumentId(normalizedId32);

            if (value is GuidString36 guidString36)
                return new DocumentId(guidString36);

            if (value is Guid guid)
                return new DocumentId(new GuidString36(guid));

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is DocumentId documentId)
            {
                if (destinationType == typeof(string))
                    return documentId.ToString();

                if (destinationType == typeof(NormalizedId32))
                    return documentId.Value;

                if (destinationType == typeof(GuidString36))
                    return documentId.Value.ToGuidString36();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DocumentIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(DocumentId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert null value to DocumentId.");
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (String.IsNullOrWhiteSpace(value))
                {
                    if (isNullable)
                        return null;

                    throw new JsonSerializationException("Cannot convert empty value to DocumentId.");
                }

                return new DocumentId(value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (String.IsNullOrWhiteSpace(value))
                    throw new JsonSerializationException("Cannot convert empty value to GuidString36.");

                return new DocumentId(new GuidString36(value));
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing DocumentId. Expected String or Guid.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((DocumentId)value).ToString());
        }
    }
}