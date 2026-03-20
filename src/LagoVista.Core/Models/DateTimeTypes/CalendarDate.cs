using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace LagoVista.Core
{
    [TypeConverter(typeof(CalendarDateTypeConverter))]
    [JsonConverter(typeof(CalendarDateJsonConverter))]
    public struct CalendarDate : IEquatable<CalendarDate>, IComparable<CalendarDate>, IComparable
    {
        private static readonly string[] AcceptedFormats = new[]
        {
            "yyyy/MM/dd",
            "yyyy-MM-dd"
        };

        private const string CanonicalFormat = "yyyy-MM-dd";

        private readonly string _value;

        public string Value => _value;

        public CalendarDate(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length != 10)
                throw new FormatException($"Invalid CalendarDate: '{value}'. Expected exactly 10 characters in yyyy/MM/dd or yyyy-MM-dd.");

            DateTime dt;
            if (!DateTime.TryParseExact(value, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                throw new FormatException($"Invalid CalendarDate: '{value}'. Expected exactly 10 characters in yyyy/MM/dd or yyyy-MM-dd.");
            }

            _value = dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture);
        }

        public string ToIsoString()
        {
            return _value;
        }

        public DateTime ToDateTime()
        {
            return DateTime.ParseExact(_value, CanonicalFormat, CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
        }

        public int Year => ToDateTime().Year;
        public int Month => ToDateTime().Month;
        public int Day => ToDateTime().Day;

        public static CalendarDate Today()
        {
            return new CalendarDate(DateTime.Today.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate StartOfMonth(int year, int month)
        {
            var dt = new DateTime(year, month, 1);
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public CalendarDate AddDays(int days)
        {
            var dt = ToDateTime().AddDays(days);
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate EndOfMonth(int year, int month)
        {
            var dt = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate Create(int year, int month, int day)
        {
            var dt = new DateTime(year, month, day);
            return new CalendarDate(dt.ToString(CanonicalFormat, CultureInfo.InvariantCulture));
        }

        public static CalendarDate Parse(string value)
        {
            return new CalendarDate(value);
        }

        [JsonIgnore]
        public CalendarDate StartOfThisMonth
        {
            get { return CalendarDate.StartOfMonth(Year, Month); }
        }

        [JsonIgnore]
        public CalendarDate EndOfThisMonth
        {
            get { return CalendarDate.EndOfMonth(Year, Month); }
        }

        [JsonIgnore]
        public CalendarDate StartOfNextMonth
        {
            get
            {
                if (Month == 12)
                    return CalendarDate.StartOfMonth(Year + 1, 1);

                return CalendarDate.StartOfMonth(Year, Month + 1);
            }
        }

        [JsonIgnore]
        public CalendarDate SameMonthNextYearStart
        {
            get { return CalendarDate.StartOfMonth(Year + 1, Month); }
        }

        public int DaysUntilInclusive(CalendarDate end)
        {
            return InclusiveDayCount(this, end);
        }

        public static int InclusiveDayCount(CalendarDate start, CalendarDate end)
        {
            if (end < start)
                throw new ArgumentException("End date must be greater than or equal to start date.", nameof(end));

            return (end.ToDateTime() - start.ToDateTime()).Days + 1;
        }

        public static bool operator >(CalendarDate left, CalendarDate right) => left.CompareTo(right) > 0;
        public static bool operator <(CalendarDate left, CalendarDate right) => left.CompareTo(right) < 0;
        public static bool operator >=(CalendarDate left, CalendarDate right) => left.CompareTo(right) >= 0;
        public static bool operator <=(CalendarDate left, CalendarDate right) => left.CompareTo(right) <= 0;
        public static bool operator ==(CalendarDate left, CalendarDate right) => left.Equals(right);
        public static bool operator !=(CalendarDate left, CalendarDate right) => !left.Equals(right);

        public override string ToString() => _value;

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public bool Equals(CalendarDate other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is CalendarDate other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public int CompareTo(CalendarDate other) => ToDateTime().CompareTo(other.ToDateTime());

        public int CompareTo(object obj)
        {
            if (!(obj is CalendarDate other))
                throw new ArgumentException("Object is not a CalendarDate", nameof(obj));

            return CompareTo(other);
        }

        public static implicit operator string(CalendarDate date) => date._value;

#if MIGRATION_IMPLICIT_WIRE_TYPES
        public static implicit operator CalendarDate(string value) => new CalendarDate(value);
#else
        public static explicit operator CalendarDate(string value) => new CalendarDate(value);
#endif

        public static bool TryCreate(string value, out CalendarDate result)
        {
            try
            {
                result = new CalendarDate(value);
                return true;
            }
            catch
            {
                result = default(CalendarDate);
                return false;
            }
        }
    }

    public class CalendarDateTypeConverter : TypeConverter
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
                throw new NotSupportedException("Cannot convert null to CalendarDate.");

            if (value is string str)
                return new CalendarDate(str);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is CalendarDate date)
                return date.Value;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class CalendarDateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return targetType == typeof(CalendarDate);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert null value to CalendarDate.");
            }

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing CalendarDate. Expected String.");

            var value = reader.Value?.ToString();

            if (String.IsNullOrWhiteSpace(value))
            {
                if (isNullable)
                    return null;

                throw new JsonSerializationException("Cannot convert empty value to CalendarDate.");
            }

            return new CalendarDate(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((CalendarDate)value).Value);
        }
    }
}