using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace LagoVista.Core.AutoMapper
{
    public sealed class EntityHeaderEnumToStringConverter : IMapValueConverter
    {
        private readonly ConcurrentDictionary<Type, object> _enumMaps =
            new ConcurrentDictionary<Type, object>();

        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt != typeof(string))
                return false;

            Type enumType;
            return TryGetEntityHeaderEnumType(st, out enumType);
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var st = sourceValue.GetType();

            Type enumType;
            if (!TryGetEntityHeaderEnumType(st, out enumType))
                throw new InvalidOperationException("Source type is not EntityHeader<TEnum>.");

            // Read the enum value from EntityHeader<TEnum>.Value
            var valueProp = st.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            if (valueProp == null)
                throw new InvalidOperationException("EntityHeader<" + enumType.Name + "> is missing Value property.");

            var enumValue = valueProp.GetValue(sourceValue);
            if (enumValue == null)
                return null;

            var map = GetOrBuildEnumMap(enumType);
            var code = map.GetCodeForEnumValue(enumValue);

            if (String.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("No EnumLabel code found for enum value " + enumType.Name + "." + enumValue + ".");

            return code;
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

        private sealed class EnumMap
        {
            private readonly Dictionary<object, string> _enumToCode;

            private EnumMap(Dictionary<object, string> enumToCode)
            {
                _enumToCode = enumToCode;
            }

            public string GetCodeForEnumValue(object enumValue)
            {
                string code;
                if (_enumToCode.TryGetValue(enumValue, out code))
                    return code;

                return null;
            }

            public static EnumMap Build(Type enumType)
            {
                var dict = new Dictionary<object, string>();

                var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                for (var i = 0; i < fields.Length; i++)
                {
                    var f = fields[i];
                    var attr = f.GetCustomAttribute<EnumLabelAttribute>(inherit: false);
                    if (attr == null)
                        continue;

                    var code = typeof(StringToEntityHeaderEnumConverter).Assembly; // placeholder to avoid circular; see note below

                    // We’ll use the same reflection helper approach as the other converter,
                    // but implemented locally to keep files independent.
                    var labelCode = GetEnumLabelCode(attr);
                    if (String.IsNullOrWhiteSpace(labelCode))
                        continue;

                    var enumValue = Enum.Parse(enumType, f.Name);
                    dict[enumValue] = labelCode;
                }

                return new EnumMap(dict);
            }

            private static string GetEnumLabelCode(EnumLabelAttribute attr)
            {
                var prop = attr.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance)
                           ?? attr.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)
                           ?? attr.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)
                           ?? attr.GetType().GetProperty("EnumValue", BindingFlags.Public | BindingFlags.Instance);

                if (prop != null && prop.PropertyType == typeof(string))
                    return (string)prop.GetValue(attr);

                var field = attr.GetType().GetField("_key", BindingFlags.NonPublic | BindingFlags.Instance)
                            ?? attr.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null && field.FieldType == typeof(string))
                    return (string)field.GetValue(attr);

                throw new InvalidOperationException("Could not read EnumLabelAttribute code. Confirm EnumLabelAttribute exposes the code as a string property (e.g., Key).");
            }
        }
    }
}