using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AI.Models.Rag
{
    internal static class RagPayloadMapper
    {
        public static Dictionary<string, object> ToDictionary(object meta, object extra)
        {
            var root = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var metaBucket = ToBucket(meta);
            var extraBucket = ToBucket(extra);

            if (metaBucket.Count > 0)
            {
                root[RagVectorPayload.BucketMeta] = metaBucket;
            }

            if (extraBucket.Count > 0)
            {
                root[RagVectorPayload.BucketExtra] = extraBucket;
            }

            return root;
        }

        public static T FromBucket<T>(IDictionary<string, object> source) where T : new()
        {
            var target = new T();

            if (source == null)
            {
                return target;
            }

            Populate(target, source);
            return target;
        }

        public static IDictionary<string, object> GetBucket(IDictionary<string, object> source, string bucketName)
        {
            if (source == null || String.IsNullOrWhiteSpace(bucketName))
            {
                return null;
            }

            foreach (var pair in source)
            {
                if (!String.Equals(pair.Key, bucketName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (pair.Value is IDictionary<string, object> dictionary)
                {
                    return dictionary;
                }

                return null;
            }

            return null;
        }

        public static TTarget CopySharedProperties<TSource, TTarget>(TSource source) where TTarget : new()
        {
            var target = new TTarget();

            if (source == null)
            {
                return target;
            }

            var sourceProperties = typeof(TSource)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
                .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var targetProperty in typeof(TTarget).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!targetProperty.CanWrite || targetProperty.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (!sourceProperties.TryGetValue(targetProperty.Name, out var sourceProperty))
                {
                    continue;
                }

                var value = sourceProperty.GetValue(source);
                var copiedValue = CloneSharedValue(value, targetProperty.PropertyType);

                if (copiedValue != null || CanAssignNull(targetProperty.PropertyType))
                {
                    targetProperty.SetValue(target, copiedValue);
                }
            }

            return target;
        }

        private static Dictionary<string, object> ToBucket(object source)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (source == null)
            {
                return result;
            }

            foreach (var property in source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanRead || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                var value = property.GetValue(source);

                if (!ShouldInclude(value))
                {
                    continue;
                }

                result[property.Name] = NormalizeForStorage(value);
            }

            return result;
        }

        private static void Populate(object target, IDictionary<string, object> source)
        {
            var sourceValues = source.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

            foreach (var property in target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanWrite || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (!sourceValues.TryGetValue(property.Name, out var rawValue))
                {
                    continue;
                }

                if (TryConvertValue(rawValue, property.PropertyType, out var convertedValue))
                {
                    property.SetValue(target, convertedValue);
                }
            }
        }

        private static bool ShouldInclude(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return !String.IsNullOrWhiteSpace(stringValue);
            }

            if (value is IEnumerable enumerable && !(value is string))
            {
                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        private static object NormalizeForStorage(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("o");
            }

            var valueType = value.GetType();

            if (valueType.IsEnum)
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }

            if (value is IEnumerable enumerable && !(value is string))
            {
                var values = new List<object>();

                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        values.Add(NormalizeForStorage(item));
                    }
                }

                return values;
            }

            return value;
        }

        private static bool TryConvertValue(object rawValue, Type targetType, out object convertedValue)
        {
            convertedValue = null;

            if (rawValue == null)
            {
                return CanAssignNull(targetType);
            }

            var nullableType = Nullable.GetUnderlyingType(targetType);
            var effectiveType = nullableType ?? targetType;

            if (effectiveType.IsInstanceOfType(rawValue))
            {
                convertedValue = rawValue;
                return true;
            }

            if (effectiveType == typeof(string))
            {
                convertedValue = rawValue.ToString();
                return true;
            }

            if (effectiveType == typeof(DateTime))
            {
                if (rawValue is DateTime dateTime)
                {
                    convertedValue = dateTime;
                    return true;
                }

                if (DateTime.TryParse(rawValue.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsedDateTime))
                {
                    convertedValue = parsedDateTime;
                    return true;
                }

                return false;
            }

            if (effectiveType.IsEnum)
            {
                try
                {
                    if (rawValue is string enumText)
                    {
                        convertedValue = Enum.Parse(effectiveType, enumText, true);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }

                try
                {
                    var numericValue = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
                    convertedValue = Enum.ToObject(effectiveType, numericValue);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            if (TryConvertCollection(rawValue, effectiveType, out convertedValue))
            {
                return true;
            }

            try
            {
                convertedValue = Convert.ChangeType(rawValue, effectiveType, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryConvertCollection(object rawValue, Type targetType, out object convertedValue)
        {
            convertedValue = null;

            if (!targetType.IsGenericType || targetType.GetGenericTypeDefinition() != typeof(List<>))
            {
                return false;
            }

            if (!(rawValue is IEnumerable enumerable) || rawValue is string)
            {
                return false;
            }

            var itemType = targetType.GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance(targetType);

            foreach (var item in enumerable)
            {
                if (TryConvertValue(item, itemType, out var convertedItem))
                {
                    list.Add(convertedItem);
                }
            }

            convertedValue = list;
            return true;
        }

        private static object CloneSharedValue(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType.IsInstanceOfType(value) && !(value is IEnumerable) || value is string)
            {
                return value;
            }

            if (TryConvertCollection(value, targetType, out var copiedCollection))
            {
                return copiedCollection;
            }

            return TryConvertValue(value, targetType, out var convertedValue) ? convertedValue : null;
        }

        private static bool CanAssignNull(Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }
    }
}
