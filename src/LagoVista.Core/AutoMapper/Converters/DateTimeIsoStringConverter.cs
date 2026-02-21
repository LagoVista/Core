using LagoVista.Core.Interfaces.AutoMapper;
using System;

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
}
