using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Tests.UIMetaData
{
    [TestClass]
    public class FormFieldMetadataCompatibilityTests
    {
        [TestMethod]
        public void ShouldAcceptConfiguredFieldType()
        {
            var entries = new[]
            {
                new FormFieldMetadataInventoryEntry
                {
                    AssemblyName = "Sample",
                    ModelType = "Sample.Device",
                    PropertyName = "Name",
                    ClrType = "System.String",
                    ClrTypeFamily = "String",
                    FieldType = FieldTypes.Text
                }
            };

            var rules = new[]
            {
                new FormFieldCompatibilityRule
                {
                    ClrTypeFamily = "String",
                    AllowedFieldTypes = new[] { FieldTypes.Text, FieldTypes.MultiLineText }
                }
            };

            Assert.AreEqual(0, FormFieldMetadataCompatibility.Validate(entries, rules).Count);
        }

        [TestMethod]
        public void ShouldPreferExactClrTypeRuleOverFamilyRule()
        {
            var entries = new[]
            {
                new FormFieldMetadataInventoryEntry
                {
                    AssemblyName = "Sample",
                    ModelType = "Sample.Workflow",
                    PropertyName = "Transitions",
                    ClrType = "List<Sample.StatusTransition>",
                    ClrTypeFamily = "List",
                    FieldType = FieldTypes.Text
                }
            };

            var rules = new[]
            {
                new FormFieldCompatibilityRule
                {
                    ClrTypeFamily = "List",
                    AllowedFieldTypes = new[] { FieldTypes.Text, FieldTypes.ChildList }
                },
                new FormFieldCompatibilityRule
                {
                    ClrType = "List<Sample.StatusTransition>",
                    AllowedFieldTypes = new[] { FieldTypes.ChildList }
                }
            };

            var issue = FormFieldMetadataCompatibility.Validate(entries, rules).Single();
            Assert.AreEqual("exact", issue.MatchedRuleScope);
            StringAssert.Contains(issue.Diagnostic, "Rule scope: exact");
            StringAssert.Contains(issue.Diagnostic, "Allowed: ChildList");
        }

        [TestMethod]
        public void ShouldAllowExactClrTypeException()
        {
            var entries = new[]
            {
                new FormFieldMetadataInventoryEntry
                {
                    AssemblyName = "Sample",
                    ModelType = "Sample.Workflow",
                    PropertyName = "Labels",
                    ClrType = "List<Sample.Label>",
                    ClrTypeFamily = "List",
                    FieldType = FieldTypes.Text
                }
            };

            var rules = new[]
            {
                new FormFieldCompatibilityRule
                {
                    ClrTypeFamily = "List",
                    AllowedFieldTypes = new[] { FieldTypes.ChildList }
                },
                new FormFieldCompatibilityRule
                {
                    ClrType = "List<Sample.Label>",
                    AllowedFieldTypes = new[] { FieldTypes.Text }
                }
            };

            Assert.AreEqual(0, FormFieldMetadataCompatibility.Validate(entries, rules).Count);
        }

        [TestMethod]
        public void ShouldReportUnsupportedPairWithUsefulDiagnostic()
        {
            var entries = new[]
            {
                new FormFieldMetadataInventoryEntry
                {
                    AssemblyName = "Sample",
                    ModelType = "Sample.Invoice",
                    PropertyName = "Total",
                    ClrType = "System.Decimal",
                    ClrTypeFamily = "Decimal",
                    FieldType = FieldTypes.EntityHeaderPicker
                }
            };

            var rules = new[]
            {
                new FormFieldCompatibilityRule
                {
                    ClrTypeFamily = "Decimal",
                    AllowedFieldTypes = new[] { FieldTypes.Decimal, FieldTypes.Money, FieldTypes.Percent }
                }
            };

            var issue = FormFieldMetadataCompatibility.Validate(entries, rules).Single();
            StringAssert.Contains(issue.Diagnostic, "FORM003");
            StringAssert.Contains(issue.Diagnostic, "Sample.Invoice.Total");
            StringAssert.Contains(issue.Diagnostic, "EntityHeaderPicker");
            StringAssert.Contains(issue.Diagnostic, "Rule scope: family");
            StringAssert.Contains(issue.Diagnostic, "Decimal, Money, Percent");
        }

        [TestMethod]
        public void ShouldReportUnconfiguredClrFamily()
        {
            var entries = new[]
            {
                new FormFieldMetadataInventoryEntry
                {
                    AssemblyName = "Sample",
                    ModelType = "Sample.Device",
                    PropertyName = "Location",
                    ClrType = "Sample.Location",
                    ClrTypeFamily = "Location",
                    FieldType = FieldTypes.GeoLocation
                }
            };

            var issue = FormFieldMetadataCompatibility.Validate(entries, new List<FormFieldCompatibilityRule>()).Single();
            StringAssert.Contains(issue.Diagnostic, "Rule scope: none");
            StringAssert.Contains(issue.Diagnostic, "Allowed: <none configured>");
        }
    }
}
