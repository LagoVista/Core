using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Models.UIMetaData
{
    public sealed class FormFieldMetadataInventoryEntry
    {
        public string AssemblyName { get; set; }
        public string ModelType { get; set; }
        public string PropertyName { get; set; }
        public string ClrType { get; set; }
        public string ClrTypeFamily { get; set; }
        public bool IsNullable { get; set; }
        public FieldTypes FieldType { get; set; }

        public string PairKey => $"{ClrTypeFamily}/{FieldType}";
    }

    public sealed class FormFieldMetadataPairSummary
    {
        public string ClrTypeFamily { get; set; }
        public FieldTypes FieldType { get; set; }
        public int Count { get; set; }
        public string PairKey => $"{ClrTypeFamily}/{FieldType}";
    }

    /// <summary>
    /// Deterministically inventories the CLR property type and FormField FieldType
    /// selected by the authoritative C# metadata attributes. This is intentionally
    /// independent of Angular and does not construct or mutate FormField instances.
    /// </summary>
    public static class FormFieldMetadataInventory
    {
        public static IReadOnlyList<FormFieldMetadataInventoryEntry> ScanAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            return ScanTypes(assembly.DefinedTypes.Select(type => type.AsType()));
        }

        public static IReadOnlyList<FormFieldMetadataInventoryEntry> ScanAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            return ScanTypes(assemblies.Where(assembly => assembly != null)
                                       .SelectMany(assembly => assembly.DefinedTypes)
                                       .Select(type => type.AsType()));
        }

        public static IReadOnlyList<FormFieldMetadataInventoryEntry> ScanTypes(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var entries = new List<FormFieldMetadataInventoryEntry>();
            foreach (var type in types.Where(type => type != null))
            {
                foreach (var property in type.GetRuntimeProperties())
                {
                    var attribute = property.GetCustomAttribute<FormFieldAttribute>();
                    if (attribute == null) continue;

                    var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
                    var effectiveType = nullableType ?? property.PropertyType;

                    entries.Add(new FormFieldMetadataInventoryEntry
                    {
                        AssemblyName = type.Assembly.GetName().Name,
                        ModelType = type.FullName ?? type.Name,
                        PropertyName = property.Name,
                        ClrType = GetClrTypeName(property.PropertyType),
                        ClrTypeFamily = GetClrTypeFamily(effectiveType),
                        IsNullable = nullableType != null,
                        FieldType = attribute.FieldType
                    });
                }
            }

            return entries.OrderBy(entry => entry.AssemblyName, StringComparer.Ordinal)
                          .ThenBy(entry => entry.ModelType, StringComparer.Ordinal)
                          .ThenBy(entry => entry.PropertyName, StringComparer.Ordinal)
                          .ToList();
        }

        public static IReadOnlyList<FormFieldMetadataPairSummary> SummarizePairs(IEnumerable<FormFieldMetadataInventoryEntry> entries)
        {
            if (entries == null) throw new ArgumentNullException(nameof(entries));

            return entries.GroupBy(entry => new { entry.ClrTypeFamily, entry.FieldType })
                          .Select(group => new FormFieldMetadataPairSummary
                          {
                              ClrTypeFamily = group.Key.ClrTypeFamily,
                              FieldType = group.Key.FieldType,
                              Count = group.Count()
                          })
                          .OrderBy(summary => summary.ClrTypeFamily, StringComparer.Ordinal)
                          .ThenBy(summary => summary.FieldType.ToString(), StringComparer.Ordinal)
                          .ToList();
        }

        /// <summary>
        /// Returns the structural CLR family used by compatibility rules. Exact generic
        /// element types remain available in ClrType for diagnostics, while collections
        /// intentionally collapse to List or Array so the compatibility matrix describes
        /// value shape rather than every domain model type carried by that collection.
        /// </summary>
        public static string GetClrTypeFamily(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type == typeof(string)) return "String";
            if (type == typeof(bool)) return "Boolean";
            if (type == typeof(byte) || type == typeof(sbyte)) return "Byte";
            if (type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)) return "Integer";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "Decimal";
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "DateTime";
            if (type.GetTypeInfo().IsEnum) return "Enum";
            if (type.IsArray) return "Array";

            if (type.GetTypeInfo().IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                if (genericDefinition == typeof(List<>) || genericDefinition == typeof(IList<>) || genericDefinition == typeof(IEnumerable<>))
                    return "List";

                return genericDefinition.Name.Split('`')[0];
            }

            return type.Name;
        }

        private static string GetClrTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return $"{GetClrTypeName(nullableType)}?";

            if (type.IsArray)
                return $"{GetClrTypeName(type.GetElementType())}[]";

            if (type.GetTypeInfo().IsGenericType)
            {
                var genericName = type.Name.Split('`')[0];
                var arguments = String.Join(",", type.GenericTypeArguments.Select(GetClrTypeName));
                return $"{genericName}<{arguments}>";
            }

            return type.FullName ?? type.Name;
        }
    }
}
