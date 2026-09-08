using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Models.UIMetaData
{
    public sealed class FormFieldCompatibilityRule
    {
        public string ClrTypeFamily { get; set; }
        public IReadOnlyCollection<FieldTypes> AllowedFieldTypes { get; set; }
    }

    public sealed class FormFieldCompatibilityIssue
    {
        public string AssemblyName { get; set; }
        public string ModelType { get; set; }
        public string PropertyName { get; set; }
        public string ClrType { get; set; }
        public string ClrTypeFamily { get; set; }
        public FieldTypes FieldType { get; set; }
        public IReadOnlyCollection<FieldTypes> AllowedFieldTypes { get; set; }

        public string Diagnostic
        {
            get
            {
                var allowed = AllowedFieldTypes == null || AllowedFieldTypes.Count == 0
                    ? "<none configured>"
                    : String.Join(", ", AllowedFieldTypes.OrderBy(fieldType => fieldType.ToString()).Select(fieldType => fieldType.ToString()));

                return $"FORM003 {ModelType}.{PropertyName}: CLR type {ClrType} ({ClrTypeFamily}) uses FieldType {FieldType}. Allowed: {allowed}.";
            }
        }
    }

    public static class FormFieldMetadataCompatibility
    {
        public static IReadOnlyList<FormFieldCompatibilityIssue> Validate(
            IEnumerable<FormFieldMetadataInventoryEntry> entries,
            IEnumerable<FormFieldCompatibilityRule> rules)
        {
            if (entries == null) throw new ArgumentNullException(nameof(entries));
            if (rules == null) throw new ArgumentNullException(nameof(rules));

            var ruleLookup = rules.ToDictionary(
                rule => rule.ClrTypeFamily,
                rule => new HashSet<FieldTypes>(rule.AllowedFieldTypes ?? Array.Empty<FieldTypes>()),
                StringComparer.Ordinal);

            var issues = new List<FormFieldCompatibilityIssue>();
            foreach (var entry in entries)
            {
                HashSet<FieldTypes> allowed;
                var hasRule = ruleLookup.TryGetValue(entry.ClrTypeFamily, out allowed);
                if (hasRule && allowed.Contains(entry.FieldType)) continue;

                issues.Add(new FormFieldCompatibilityIssue
                {
                    AssemblyName = entry.AssemblyName,
                    ModelType = entry.ModelType,
                    PropertyName = entry.PropertyName,
                    ClrType = entry.ClrType,
                    ClrTypeFamily = entry.ClrTypeFamily,
                    FieldType = entry.FieldType,
                    AllowedFieldTypes = hasRule
                        ? allowed.OrderBy(fieldType => fieldType.ToString()).ToList()
                        : Array.Empty<FieldTypes>()
                });
            }

            return issues.OrderBy(issue => issue.AssemblyName, StringComparer.Ordinal)
                         .ThenBy(issue => issue.ModelType, StringComparer.Ordinal)
                         .ThenBy(issue => issue.PropertyName, StringComparer.Ordinal)
                         .ToList();
        }
    }
}
