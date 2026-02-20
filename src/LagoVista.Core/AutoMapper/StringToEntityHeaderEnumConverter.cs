using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AutoMapper
{
    public sealed class StringToEntityHeaderEnumConverter : IMapValueConverter
    {
        private readonly ConcurrentDictionary<Type, object> _enumMaps =
            new ConcurrentDictionary<Type, object>();

        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (st != typeof(string))
                return false;

            // EntityHeader<TEnum>
            return TryGetEntityHeaderEnumType(tt, out _);
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var code = sourceValue as string;
            if (code == null)
                throw new InvalidOperationException("Expected string source value.");

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            Type enumType;
            if (!TryGetEntityHeaderEnumType(tt, out enumType))
                throw new InvalidOperationException("Target type is not EntityHeader<TEnum>.");

            var map = GetOrBuildEnumMap(enumType);

            if (String.IsNullOrWhiteSpace(code))
            {
                // If the target is nullable, allow null. Otherwise, still return null (EntityHeader is ref-type).
                return null;
            }

            object enumValue;
            if (!map.TryGetEnumValue(code, out enumValue))
            {
                throw new InvalidOperationException(
                    "Could not map code '" + code + "' to enum " + enumType.Name +
                    ". Ensure the enum value has [EnumLabel(\"" + code + "\", ...)].");
            }

            return CreateEntityHeader(enumType, enumValue);
        }

        private bool TryGetEntityHeaderEnumType(Type type, out Type enumType)
        {
            enumType = null;

            if (!type.IsGenericType)
                return false;

            var def = type.GetGenericTypeDefinition();
            if (def != typeof(EntityHeader<>))
                return false;

            var args = type.GetGenericArguments();
            if (args.Length != 1)
                return false;

            enumType = args[0];
            return enumType.IsEnum;
        }

        private EnumMap GetOrBuildEnumMap(Type enumType)
        {
            object existing;
            if (_enumMaps.TryGetValue(enumType, out existing))
                return (EnumMap)existing;

            var map = EnumMap.Build(enumType);
            _enumMaps[enumType] = map;
            return map;
        }

        private object CreateEntityHeader(Type enumType, object enumValue)
        {
            // EntityHeader<TEnum>.Create(TEnum value)
            var ehType = typeof(EntityHeader<>).MakeGenericType(enumType);
            var createMethod = ehType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "Create" &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == enumType);

            if (createMethod == null)
                throw new InvalidOperationException("Could not find EntityHeader<" + enumType.Name + ">.Create(TEnum).");

            return createMethod.Invoke(null, new[] { enumValue });
        }

        private sealed class EnumMap
        {
            private readonly Dictionary<string, object> _codeToEnum;

            private EnumMap(Dictionary<string, object> codeToEnum)
            {
                _codeToEnum = codeToEnum;
            }

            public bool TryGetEnumValue(string code, out object value)
            {
                return _codeToEnum.TryGetValue(code, out value);
            }

            public static EnumMap Build(Type enumType)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                for (var i = 0; i < fields.Length; i++)
                {
                    var f = fields[i];
                    var attr = f.GetCustomAttribute<EnumLabelAttribute>(inherit: false);
                    if (attr == null)
                        continue;

                    // EnumLabelAttribute’s first constructor arg is your stable string code.
                    // In LagoVista, that is typically stored as "Key" or similar.
                    // We’ll read it via reflection to avoid depending on internal API details.
                    var code = GetEnumLabelCode(attr);
                    if (String.IsNullOrWhiteSpace(code))
                        continue;

                    var enumValue = Enum.Parse(enumType, f.Name);

                    // Allow duplicates? No. Last one wins would hide bugs. Throw.
                    if (dict.ContainsKey(code))
                        throw new InvalidOperationException("Duplicate EnumLabel code '" + code + "' on enum " + enumType.Name + ".");

                    dict.Add(code, enumValue);
                }

                return new EnumMap(dict);
            }

            private static string GetEnumLabelCode(EnumLabelAttribute attr)
            {
                // Prefer a public property if available.
                var prop = attr.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance)
                           ?? attr.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)
                           ?? attr.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)
                           ?? attr.GetType().GetProperty("EnumValue", BindingFlags.Public | BindingFlags.Instance);

                if (prop != null && prop.PropertyType == typeof(string))
                    return (string)prop.GetValue(attr);

                // Fallback: try field backing (rare)
                var field = attr.GetType().GetField("_key", BindingFlags.NonPublic | BindingFlags.Instance)
                            ?? attr.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null && field.FieldType == typeof(string))
                    return (string)field.GetValue(attr);

                // If none found, we can’t infer. You can hardcode once you confirm the real property name.
                throw new InvalidOperationException("Could not read EnumLabelAttribute code. Confirm EnumLabelAttribute exposes the code as a string property (e.g., Key).");
            }
        }
    }
}