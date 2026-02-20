using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
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

    public sealed class EntityHeaderIdConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (st != typeof(EntityHeader))
                return false;

            return tt == typeof(string) || tt == typeof(Guid) || tt == typeof(Guid?);
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var eh = (EntityHeader)sourceValue;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt == typeof(string))
                return eh.Id;

            if (tt == typeof(Guid))
            {
                if (String.IsNullOrWhiteSpace(eh.Id))
                {
                    if (Nullable.GetUnderlyingType(targetType) != null)
                        return null;

                    throw new InvalidOperationException("Cannot convert empty EntityHeader.Id to non-nullable Guid.");
                }

                if (!Guid.TryParse(eh.Id, out var g))
                    throw new InvalidOperationException($"Could not convert EntityHeader.Id to Guid: '{eh.Id}'.");

                return g;
            }

            throw new InvalidOperationException($"Unsupported conversion from EntityHeader to {targetType.Name}.");
        }
    }

    public sealed class ToEntityHeaderConverter : IMapValueConverter
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo?> _methodCache =
            new ConcurrentDictionary<Type, MethodInfo?>();

        public bool CanConvert(Type sourceType, Type targetType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt != typeof(EntityHeader))
                return false;

            // Only convert if the source type has a public instance method:
            // EntityHeader ToEntityHeader()
            return GetToEntityHeaderMethod(st) != null;
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (tt != typeof(EntityHeader))
                throw new InvalidOperationException($"ToEntityHeaderConverter only supports converting to EntityHeader, not {targetType.Name}.");

            var st = sourceValue.GetType();
            var method = GetToEntityHeaderMethod(st);

            if (method == null)
                throw new InvalidOperationException($"Type '{st.FullName}' does not implement a compatible ToEntityHeader() method.");

            // Invoke: EntityHeader ToEntityHeader()
            var result = method.Invoke(sourceValue, parameters: null);

            if (result == null)
            {
                // Returning null is allowed only if targetType is nullable (EntityHeader is a ref type anyway)
                return null;
            }

            if (result is EntityHeader eh)
                return eh;

            // Defensive: should never happen given our checks.
            throw new InvalidOperationException(
                $"ToEntityHeader() on '{st.FullName}' returned '{result.GetType().FullName}', expected EntityHeader.");
        }

        private MethodInfo GetToEntityHeaderMethod(Type sourceType)
        {
            MethodInfo method;

            if (_methodCache.TryGetValue(sourceType, out method))
                return method;

            method = FindToEntityHeaderMethod(sourceType);

            // We cache even null results to avoid repeated reflection
            _methodCache[sourceType] = method;

            return method;
        }

        private MethodInfo FindToEntityHeaderMethod(Type sourceType)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var method = sourceType.GetMethod(
                "ToEntityHeader",
                flags,
                null,
                Type.EmptyTypes,
                null);

            if (method == null)
                return null;

            if (method.ReturnType != typeof(EntityHeader))
                return null;

            return method;
        }
    }

    public static class ConvertersStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMapValueConverter, ToEntityHeaderConverter>();
            services.AddSingleton<IMapValueConverter, EntityHeaderIdConverter>();
            services.AddSingleton<IMapValueConverter, DateTimeIsoStringConverter>();
            services.AddSingleton<IMapValueConverter, NumericStringConverter>();
            services.AddSingleton<IMapValueConverter, EntityHeaderEnumToStringConverter>();
            services.AddSingleton<IMapValueConverter, StringToEntityHeaderEnumConverter>();
            services.AddSingleton<IMapValueConverter, GuidStringConverter>();
            services.AddSingleton<IMapValueConverterRegistry, MapValueConverterRegistry>();
        }

        public static IMapValueConverterRegistry DefaultConverterRegistery = new MapValueConverterRegistry(new IMapValueConverter[]
        {
            new ToEntityHeaderConverter(),
            new EntityHeaderIdConverter(),
            new DateTimeIsoStringConverter(),
            new NumericStringConverter(),
            new GuidStringConverter(),
            new EntityHeaderEnumToStringConverter(),
            new StringToEntityHeaderEnumConverter(),
        });

    }
}
