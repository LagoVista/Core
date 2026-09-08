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
            StringAssert.Contains(issue.Diagnostic, "Allowed: <none configured>");
        }
    }
}
