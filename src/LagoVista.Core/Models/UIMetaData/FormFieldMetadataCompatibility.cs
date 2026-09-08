using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Models.UIMetaData
{
    public sealed class FormFieldCompatibilityRule
    {
        public string ClrTypeFamily { get; set; }
        public string ClrType { get; set; }
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
        public string MatchedRuleScope { get; set; }

        public string Diagnostic
        {
            get
            {
                var allowed = AllowedFieldTypes == null || AllowedFieldTypes.Count == 0
                    ? "<none configured>"
                    : String.Join(", ", AllowedFieldTypes.OrderBy(fieldType => fieldType.ToString()).Select(fieldType => fieldType.ToString()));

                var scope = String.IsNullOrWhiteSpace(MatchedRuleScope) ? "none" : MatchedRuleScope;
                return $"FORM003 {ModelType}.{PropertyName}: CLR type {ClrType} ({ClrTypeFamily}) uses FieldType {FieldType}. Rule scope: {scope}. Allowed: {allowed}.";
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

            var materializedRules = rules.ToList();
            var familyRuleLookup = materializedRules
                .Where(rule => !String.IsNullOrWhiteSpace(rule.ClrTypeFamily) && String.IsNullOrWhiteSpace(rule.ClrType))
                .ToDictionary(
                    rule => rule.ClrTypeFamily,
                    rule => new HashSet<FieldTypes>(rule.AllowedFieldTypes ?? Array.Empty<FieldTypes>()),
                    StringComparer.Ordinal);

            var exactRuleLookup = materializedRules
                .Where(rule => !String.IsNullOrWhiteSpace(rule.ClrType))
                .ToDictionary(
                    rule => rule.ClrType,
                    rule => new HashSet<FieldTypes>(rule.AllowedFieldTypes ?? Array.Empty<FieldTypes>()),
                    StringComparer.Ordinal);

            var issues = new List<FormFieldCompatibilityIssue>();
            foreach (var entry in entries)
            {
                HashSet<FieldTypes> allowed;
                string matchedRuleScope = null;

                if (exactRuleLookup.TryGetValue(entry.ClrType, out allowed))
                {
                    matchedRuleScope = "exact";
                }
                else if (familyRuleLookup.TryGetValue(entry.ClrTypeFamily, out allowed))
                {
                    matchedRuleScope = "family";
                }

                if (allowed != null && allowed.Contains(entry.FieldType)) continue;

                IReadOnlyCollection<FieldTypes> allowedFieldTypes = allowed != null
                    ? (IReadOnlyCollection<FieldTypes>)allowed.OrderBy(fieldType => fieldType.ToString()).ToList()
                    : Array.Empty<FieldTypes>();

                issues.Add(new FormFieldCompatibilityIssue
                {
                    AssemblyName = entry.AssemblyName,
                    ModelType = entry.ModelType,
                    PropertyName = entry.PropertyName,
                    ClrType = entry.ClrType,
                    ClrTypeFamily = entry.ClrTypeFamily,
                    FieldType = entry.FieldType,
                    AllowedFieldTypes = allowedFieldTypes,
                    MatchedRuleScope = matchedRuleScope
                });
            }

            return issues.OrderBy(issue => issue.AssemblyName, StringComparer.Ordinal)
                         .ThenBy(issue => issue.ModelType, StringComparer.Ordinal)
                         .ThenBy(issue => issue.PropertyName, StringComparer.Ordinal)
                         .ToList();
        }
    }
}
