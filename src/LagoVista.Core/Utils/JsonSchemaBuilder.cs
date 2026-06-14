using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Utils
{
    public static class JsonSchemaBuilder
    {
        private const int MaxDepth = 8;

        public static JObject FromType<T>()
        {
            return FromType(typeof(T));
        }

        public static JObject FromType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var visited = new HashSet<Type>();
            return BuildSchema(type, visited, 0);
        }

        private static bool IsNormalizedId32Type(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.Name == "NormalizedId32";
        }

        private static JObject BuildSchema(Type type, HashSet<Type> visited, int depth)
        {
            if (type == null)
            {
                return AnyJsonSchema("Unknown type.");
            }

            if (depth > MaxDepth)
            {
                return new JObject
                {
                    ["type"] = "object",
                    ["description"] = $"Schema depth limit reached for {type.Name}."
                };
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string) || type == typeof(Guid))
            {
                return TypeSchema("string");
            }

            if (IsNormalizedId32Type((Type)type))
            {
                return TypeSchema("string", "Random GUID-ish value, 32 characters, ONLY numbers and upper case letters, no special charaters or symbols");
            }

            if (type == typeof(bool))
            {
                return TypeSchema("boolean");
            }

            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
            {
                return TypeSchema("integer");
            }

            if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            {
                return TypeSchema("number");
            }

            if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            {
                return new JObject
                {
                    ["type"] = "string",
                    ["format"] = "date-time"
                };
            }

            if (type == typeof(object))
            {
                return AnyJsonSchema("Any valid JSON value.");
            }

            if (typeof(JToken).IsAssignableFrom(type))
            {
                return AnyJsonSchema("Any valid JSON value.");
            }

            if (type.IsEnum)
            {
                return new JObject
                {
                    ["type"] = "string",
                    ["enum"] = new JArray(Enum.GetNames(type))
                };
            }

            if (IsEntityHeaderType(type)) return EntityHeaderSchema();

            if (TryGetDictionaryValueType(type, out var dictionaryValueType))
            {
                return new JObject
                {
                    ["type"] = "object",
                    ["additionalProperties"] = BuildSchema(dictionaryValueType, visited, depth + 1)
                };
            }

            if (TryGetEnumerableItemType(type, out var itemType))
            {
                return new JObject
                {
                    ["type"] = "array",
                    ["items"] = BuildSchema(itemType, visited, depth + 1)
                };
            }

            if (ShouldTreatAsOpaqueObject(type))
            {
                return new JObject
                {
                    ["type"] = "object",
                    ["description"] = $"Opaque object of type {type.Name}."
                };
            }

            if (visited.Contains(type))
            {
                return new JObject
                {
                    ["type"] = "object",
                    ["description"] = $"Circular reference placeholder for {type.Name}."
                };
            }

            visited.Add(type);

            var properties = new JObject();
            var required = new JArray();

            foreach (var property in GetSerializableProperties(type))
            {
                var propertyName = ToCamelCase(property.Name);

                try
                {
                    var propertySchema = BuildSchema(property.PropertyType, visited, depth + 1);
                    var description = GetDescription(property);

                    if (!String.IsNullOrWhiteSpace(description))
                    {
                        propertySchema["description"] = description;
                    }

                    properties[propertyName] = propertySchema;

                    if (IsRequired(property))
                    {
                        required.Add(propertyName);
                    }
                }
                catch (Exception ex)
                {
                    properties[propertyName] = new JObject
                    {
                        ["type"] = "object",
                        ["description"] = $"Could not generate schema for property {type.Name}.{property.Name}: {ex.Message}"
                    };
                }
            }

            visited.Remove(type);

            var schema = new JObject
            {
                ["type"] = "object",
                ["properties"] = properties,
                ["additionalProperties"] = false
            };

            if (required.Count > 0)
            {
                schema["required"] = required;
            }

            return schema;
        }

        private static JObject TypeSchema(string type)
        {
            return new JObject
            {
                ["type"] = type
            };
        }

        private static JObject TypeSchema(string type, string description)
        {
            return new JObject
            {
                ["type"] = type,
                ["description"] = description   
            };
        }

        private static JObject AnyJsonSchema(string description)
        {
            return new JObject
            {
                ["description"] = description
            };
        }

        private static IEnumerable<PropertyInfo> GetSerializableProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.CanRead)
                .Where(prop => prop.GetIndexParameters().Length == 0)
                .Where(prop => !HasAttributeNamed(prop, "JsonIgnoreAttribute"));
        }

        private static bool TryGetDictionaryValueType(Type type, out Type valueType)
        {
            valueType = null;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var args = type.GetGenericArguments();

                if (args.Length == 2 && args[0] == typeof(string))
                {
                    valueType = args[1];
                    return true;
                }
            }

            var dictionaryType = type.GetInterfaces()
                .Where(iface => iface.IsGenericType)
                .FirstOrDefault(iface => iface.GetGenericTypeDefinition() == typeof(IDictionary<,>) && iface.GetGenericArguments()[0] == typeof(string));

            if (dictionaryType == null)
            {
                return false;
            }

            valueType = dictionaryType.GetGenericArguments()[1];
            return true;
        }

        private static bool TryGetEnumerableItemType(Type type, out Type itemType)
        {
            itemType = null;

            if (type == typeof(string))
            {
                return false;
            }

            if (type.IsArray)
            {
                itemType = type.GetElementType();
                return itemType != null;
            }

            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return false;
            }

            var enumerableType = type.GetInterfaces()
                .Where(iface => iface.IsGenericType)
                .FirstOrDefault(iface => iface.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerableType == null)
            {
                return false;
            }

            itemType = enumerableType.GetGenericArguments().FirstOrDefault();
            return itemType != null;
        }

        private static bool ShouldTreatAsOpaqueObject(Type type)
        {
            if (type.Assembly == typeof(string).Assembly)
            {
                return true;
            }

            if (type.Namespace != null && type.Namespace.StartsWith("System.", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        private static bool IsRequired(PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (Nullable.GetUnderlyingType(propertyType) != null)
            {
                return false;
            }

            if (!propertyType.IsValueType)
            {
                return HasAttributeNamed(property, "RequiredAttribute");
            }

            return true;
        }

        private static string GetDescription(PropertyInfo property)
        {
            var descriptionAttribute = property
                .GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute == null)
            {
                return null;
            }

            var descriptionProperty = descriptionAttribute.GetType().GetProperty("Description");

            return descriptionProperty == null
                ? null
                : descriptionProperty.GetValue(descriptionAttribute)?.ToString();
        }

        private static bool HasAttributeNamed(PropertyInfo property, string attributeName)
        {
            return property
                .GetCustomAttributes()
                .Any(attr => attr.GetType().Name == attributeName);
        }

        private static string ToCamelCase(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return value.ToLowerInvariant();
            }

            return Char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static bool IsEntityHeaderType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.FullName == "LagoVista.Core.Models.EntityHeader")
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "LagoVista.Core.Models.EntityHeader`1")
            {
                return true;
            }

            return false;
        }

        private static JObject EntityHeaderSchema()
        {
            return new JObject
            {
                ["type"] = "object",
                ["properties"] = new JObject
                {
                    ["id"] = TypeSchema("string"),
                    ["key"] = TypeSchema("string"),
                    ["text"] = TypeSchema("string")
                },
                ["additionalProperties"] = false
            };
        }
    }
}