using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Globalization;
using System.Reflection;

namespace LagoVista.Core.AutoMapper.Converters
{
    public sealed class DateTimeIsoStringConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // DateTime <-> string
            if (tt == typeof(string) && st == typeof(DateTime))
                return true;

            if (st == typeof(string) && tt == typeof(DateTime))
                return true;

            // DateTime <-> UtcTimestamp
            if (tt == typeof(UtcTimestamp) && st == typeof(DateTime))
                return true;

            if (st == typeof(UtcTimestamp) && tt == typeof(DateTime))
                return true;

            return false;
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // DateTime -> string
            if (sourceValue is DateTime dt && tt == typeof(string))
                return dt.ToJSONString();

            // string -> DateTime
            if (sourceValue is string s && tt == typeof(DateTime))
                return s.ToDateTime();

            // DateTime -> UtcTimestamp
            if (sourceValue is DateTime dt2 && tt == typeof(UtcTimestamp))
            {
                // "Loud" policy: enforce UTC semantics at boundary.
                // If you prefer to allow Unspecified and treat it as UTC, swap the throw for SpecifyKind.
                if (dt2.Kind == DateTimeKind.Unspecified)
                    throw new InvalidOperationException("DateTime.Kind is Unspecified. Expected UTC DateTime for UtcTimestamp mapping.");

                var utc = dt2.Kind == DateTimeKind.Utc ? dt2 : dt2.ToUniversalTime();
                return new UtcTimestamp(utc.ToJSONString());
            }

            // UtcTimestamp -> DateTime
            if (sourceValue is UtcTimestamp ts && tt == typeof(DateTime))
                return ts.ToDateTimeUtc();

            throw new InvalidOperationException($"Unsupported conversion from {sourceValue.GetType().Name} to {targetType.Name}.");
        }
    }

    public sealed class DateOnlyStringConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // DateOnly <-> string
            if (tt == typeof(string) && IsDateOnlyType(st))
                return true;

            if (st == typeof(string) && IsDateOnlyType(tt))
                return true;

            // DateOnly <-> CalendarDate
            if (tt == typeof(CalendarDate) && IsDateOnlyType(st))
                return true;

            if (st == typeof(CalendarDate) && IsDateOnlyType(tt))
                return true;

            return false;
        }

        private static bool IsDateOnlyType(Type t)
        {
            // DateOnly full name is "System.DateOnly" when present.
            return t.Name == "DateOnly" && t.Namespace == "System";
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // DateOnly -> string (netstandard2.0-safe)
            if (tt == typeof(string) && IsDateOnlyType(sourceValue.GetType()))
                return DateOnlyToString(sourceValue);

            // string -> DateOnly (netstandard2.0-safe)
            if (sourceValue is string s && IsDateOnlyType(tt))
            {
                // Accept your legacy yyyy/MM/dd and new yyyy-MM-dd by leveraging your existing helper,
                // but try invariant parse first for stability.
                DateTime parsed;
                if (!DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    parsed = s.ToDateTime();

                return CreateDateOnlyFromDateTime(tt, parsed);
            }

            // DateOnly -> CalendarDate
            if (tt == typeof(CalendarDate) && IsDateOnlyType(sourceValue.GetType()))
            {
                // Convert DateOnly -> canonical 10-char string, then wrap.
                var dateString = DateOnlyToString(sourceValue);
                return new CalendarDate(dateString);
            }

            // CalendarDate -> DateOnly
            if (sourceValue is CalendarDate cd && IsDateOnlyType(tt))
            {
                var dt = cd.ToDateTime();
                return CreateDateOnlyFromDateTime(tt, dt);
            }

            throw new InvalidOperationException($"Unsupported conversion from {sourceValue.GetType().Name} to {targetType.Name}.");
        }

        private static string DateOnlyToString(object dateOnly)
        {
            var type = dateOnly.GetType();

            // Prefer DateOnly.ToString(string, IFormatProvider) when available
            var toStringWithFormat = type.GetMethod(
                "ToString",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(string), typeof(IFormatProvider) },
                modifiers: null);

            if (toStringWithFormat != null)
            {
                // This will use whatever your StringExtensions.DATE_ONLY_FORMAT currently is.
                return (string)toStringWithFormat.Invoke(
                    dateOnly,
                    new object[] { StringExtensions.DATE_ONLY_FORMAT, CultureInfo.InvariantCulture });
            }

            // Fallback
            return dateOnly.ToString();
        }

        private static object CreateDateOnlyFromDateTime(Type dateOnlyType, DateTime dt)
        {
            var fromDateTime = dateOnlyType.GetMethod(
                "FromDateTime",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(DateTime) },
                modifiers: null);

            if (fromDateTime == null)
                throw new MissingMethodException(dateOnlyType.FullName, "FromDateTime(DateTime)");

            return fromDateTime.Invoke(null, new object[] { dt });
        }
    }
}
