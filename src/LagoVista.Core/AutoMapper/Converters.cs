using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static LagoVista.Core.AutoMapper.NumericStringConverter;

namespace LagoVista.Core.AutoMapper
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

    public sealed class NumericStringConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // decimal/double -> string
            if (tt == typeof(string) && (st == typeof(decimal) || st == typeof(double)))
                return true;

            // string -> decimal/double
            if (st == typeof(string) && (tt == typeof(decimal) || tt == typeof(double)))
                return true;

            return false;
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // numeric -> string
            if (sourceValue is decimal dec && tt == typeof(string))
                return dec.ToString(CultureInfo.InvariantCulture);

            if (sourceValue is double dbl && tt == typeof(string))
                return dbl.ToString(CultureInfo.InvariantCulture);

            // string -> numeric
            if (sourceValue is string s)
            {
                if (String.IsNullOrWhiteSpace(s))
                {
                    if (IsNullable(targetType))
                        return null;

                    throw new InvalidOperationException("Cannot convert empty string to non-nullable numeric type.");
                }

                if (tt == typeof(decimal))
                    return decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);

                if (tt == typeof(double))
                    return double.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException($"Unsupported numeric conversion from {sourceValue.GetType().Name} to {targetType.Name}.");
        }

        private static bool IsNullable(Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }
    }

        public sealed class GuidStringConverter : IMapValueConverter
        {
            public bool CanConvert(Type sourceType, Type targetType)
            {
                var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
                var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (tt == typeof(string) && st == typeof(Guid))
                    return true;

                if (st == typeof(string) && tt == typeof(Guid))
                    return true;

                return false;
            }

            public object Convert(object sourceValue, Type targetType)
            {
                if (sourceValue == null)
                    return null;

                var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

                // Guid -> string
                if (sourceValue is Guid g && tt == typeof(string))
                    return g.ToString("D");

                // string -> Guid / Guid?
                if (sourceValue is string s && tt == typeof(Guid))
                {
                    if (String.IsNullOrWhiteSpace(s))
                    {
                        if (IsNullable(targetType))
                            return null;

                        throw new InvalidOperationException("Cannot convert empty string to non-nullable Guid.");
                    }

                    if (!Guid.TryParse(s, out var parsed))
                        throw new InvalidOperationException($"Could not convert string to Guid: '{s}'.");

                    return parsed;
                }

                throw new InvalidOperationException($"Unsupported Guid conversion from {sourceValue.GetType().Name} to {targetType.Name}.");
            }

            private static bool IsNullable(Type type)
            {
                return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
            }
        }

    public static class ConvertersStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMapValueConverter, DateTimeIsoStringConverter>();
            services.AddSingleton<IMapValueConverter, NumericStringConverter>();
            services.AddSingleton<IMapValueConverter, GuidStringConverter>();
            services.AddSingleton<IMapValueConverterRegistry, MapValueConverterRegistry>();
        }
    }
}
