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

            if (tt == typeof(string) && (st == typeof(DateTime)))
                return true;

            if (st == typeof(string) && (tt == typeof(DateTime)))
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

            // string -> DateTime / DateTime?
            if (sourceValue is string s && tt == typeof(DateTime))
            {
                return s.ToDateTime();
            }

            throw new InvalidOperationException($"Unsupported conversion from {sourceValue.GetType().Name} to {targetType.Name}.");
        }
    }

    public sealed class DateOnlyStringConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt == typeof(string) && (IsDateOnlyType(st)))
                return true;

            if (st == typeof(string) && (IsDateOnlyType(tt)))
                return true;

            return false;
        }

        private static bool IsDateOnlyType(Type t)
        {
            // Keep it strict-ish to avoid accidental collisions.
            // DateOnly full name is "System.DateOnly" when present.
            return t.Name == "DateOnly" && t.Namespace == "System";
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // -----------------------------
            // DateTime -> string
            // -----------------------------
            if (sourceValue is DateTime dt && tt == typeof(string))
                return dt.ToJSONString();

            // -----------------------------
            // string -> DateTime / DateTime?
            // -----------------------------
            if (sourceValue is string s1 && tt == typeof(DateTime))
                return s1.ToDateTime();

            // -----------------------------
            // DateOnly -> string (netstandard2.0-safe)
            // -----------------------------
            if (tt == typeof(string) && IsDateOnlyType(sourceValue.GetType()))
                return DateOnlyToIsoString(sourceValue);

            // -----------------------------
            // string -> DateOnly (netstandard2.0-safe)
            // -----------------------------
            if (sourceValue is string s2 && IsDateOnlyType(tt))
            {
                // If you already trust s2.ToDateTime(), you can keep it.
                // I recommend invariant parsing for stability with DB-like values.
                if (!DateTime.TryParse(s2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    // Fall back to your existing helper if it supports more formats
                    parsed = s2.ToDateTime();
                }

                return CreateDateOnlyFromDateTime(tt, parsed);
            }

            throw new InvalidOperationException($"Unsupported conversion from {sourceValue.GetType().Name} to {targetType.Name}.");
        }


        private static string DateOnlyToIsoString(object dateOnly)
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
