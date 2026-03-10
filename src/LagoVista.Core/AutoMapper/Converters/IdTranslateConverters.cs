using LagoVista.Core.Interfaces.AutoMapper;
using System;

namespace LagoVista.Core.AutoMapper.Converters
{
    [CriticalCoverage]
    public sealed class IdTranslateConverters : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt == typeof(string) && st == typeof(NormalizedId32))
                return true;

            if (tt == typeof(string) && st == typeof(Guid))
                return true;

            if (st == typeof(string) && tt == typeof(Guid))
                return true;


            if (st == typeof(string) && tt == typeof(NormalizedId32))
                return true;

            if (st == typeof(NormalizedId32) && tt == typeof(Guid))
                return true;

            if (st == typeof(GuidString36) && tt == typeof(Guid))
                return true;

            if (st == typeof(Guid) && tt == typeof(GuidString36))
                return true;

            return false;
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var st = Nullable.GetUnderlyingType(sourceValue.GetType()) ?? sourceValue.GetType();

            // Guid -> string
            if (sourceValue is Guid g && tt == typeof(string))
                return g.ToString("D");

            if (st == typeof(GuidString36) && tt == typeof(Guid))
            {
                return ((GuidString36)sourceValue).ToGuid();
            }

            if(sourceValue is Guid g2 && tt == typeof(GuidString36))
                return new GuidString36(g2);

            if (sourceValue is NormalizedId32 && tt == typeof(string))
                return sourceValue.ToString();


            if (sourceValue is string && tt == typeof(NormalizedId32))
                return new NormalizedId32(sourceValue.ToString());

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

            if (sourceValue is NormalizedId32 ng && tt == typeof(Guid))
            {
                if (String.IsNullOrWhiteSpace(ng))
                {
                    if (IsNullable(targetType))
                        return null;

                    throw new InvalidOperationException("Cannot convert empty string to non-nullable Guid.");
                }

                if (!Guid.TryParse(ng.Value, out var parsed))
                    throw new InvalidOperationException($"Could not convert string to Guid: '{ng.Value}'.");

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
