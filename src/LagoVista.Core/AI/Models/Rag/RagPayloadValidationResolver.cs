using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    internal static class RagPayloadValidationResolver
    {
        public static void ValidateAndNormalize(object target, InvokeResult result)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            ApplyDefaults(target, properties, result);
            ValidateRequired(target, properties, result);
            ValidateNotDefault(target, properties, result);
            ValidateMinimums(target, properties, result);
        }

        private static void ApplyDefaults(object target, IEnumerable<PropertyInfo> properties, InvokeResult result)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<RagDefaultValueAttribute>(true);

                if (attribute == null || !property.CanWrite)
                {
                    continue;
                }

                var value = property.GetValue(target);

                if (!IsMissingOrInvalid(value, property.PropertyType))
                {
                    continue;
                }

                property.SetValue(target, ConvertValue(attribute.Value, property.PropertyType));

                if (!String.IsNullOrWhiteSpace(attribute.Warning))
                {
                    result.AddWarning(attribute.Warning);
                }
            }
        }

        private static void ValidateRequired(object target, IEnumerable<PropertyInfo> properties, InvokeResult result)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<RagRequiredAttribute>(true);

                if (attribute == null)
                {
                    continue;
                }

                var value = property.GetValue(target);

                if (!IsMissing(value))
                {
                    continue;
                }

                result.AddUserError(attribute.Message ?? $"{property.Name} is required.");
            }
        }

        private static void ValidateNotDefault(object target, IEnumerable<PropertyInfo> properties, InvokeResult result)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<RagNotDefaultAttribute>(true);

                if (attribute == null)
                {
                    continue;
                }

                var value = property.GetValue(target);
                var defaultValue = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;

                if (!Equals(value, defaultValue))
                {
                    continue;
                }

                result.AddUserError(attribute.Message ?? $"{property.Name} must be specified.");
            }
        }

        private static void ValidateMinimums(object target, IEnumerable<PropertyInfo> properties, InvokeResult result)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<RagMinimumAttribute>(true);

                if (attribute == null)
                {
                    continue;
                }

                var value = property.GetValue(target);

                if (value == null)
                {
                    continue;
                }

                var numericValue = Convert.ToInt64(value);

                if (numericValue < attribute.Minimum)
                {
                    result.AddUserError($"{property.Name} cannot be less than {attribute.Minimum}.");
                }
            }
        }

        private static bool IsMissing(object value)
        {
            return value == null || value is string stringValue && String.IsNullOrWhiteSpace(stringValue);
        }

        private static bool IsMissingOrInvalid(object value, Type propertyType)
        {
            if (IsMissing(value))
            {
                return true;
            }

            if (propertyType == typeof(int))
            {
                return (int)value <= 0;
            }

            if (propertyType == typeof(long))
            {
                return (long)value <= 0;
            }

            return false;
        }

        private static object ConvertValue(object value, Type targetType)
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value.ToString(), true);
            }

            return Convert.ChangeType(value, underlyingType);
        }
    }
}
