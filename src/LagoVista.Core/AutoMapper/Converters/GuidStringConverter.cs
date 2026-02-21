using LagoVista.Core.Interfaces.AutoMapper;
using System;

namespace LagoVista.Core.AutoMapper.Converters
{
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
}
