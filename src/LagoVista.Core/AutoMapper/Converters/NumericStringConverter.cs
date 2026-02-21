using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Globalization;

namespace LagoVista.Core.AutoMapper
{
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
}
