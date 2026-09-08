using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LagoVista.Core.Tests.UIMetaData
{
    [TestClass]
    public class FormFieldMetadataInventoryTests
    {
        private class SampleFormModel
        {
            [FormField(FieldType = FieldTypes.Text)]
            public string Name { get; set; }

            [FormField(FieldType = FieldTypes.Integer)]
            public int RetryCount { get; set; }

            [FormField(FieldType = FieldTypes.Decimal)]
            public decimal? Threshold { get; set; }

            [FormField(FieldType = FieldTypes.DateTime)]
            public DateTime CreatedUtc { get; set; }

            public string NotAFormField { get; set; }
        }

        [TestMethod]
        public void ShouldInventoryFormFieldClrTypeAndFieldType()
        {
            var entries = FormFieldMetadataInventory.ScanTypes(new[] { typeof(SampleFormModel) });

            Assert.AreEqual(4, entries.Count);

            var name = entries.Single(entry => entry.PropertyName == nameof(SampleFormModel.Name));
            Assert.AreEqual("String", name.ClrTypeFamily);
            Assert.AreEqual(FieldTypes.Text, name.FieldType);
            Assert.IsFalse(name.IsNullable);

            var threshold = entries.Single(entry => entry.PropertyName == nameof(SampleFormModel.Threshold));
            Assert.AreEqual("Decimal", threshold.ClrTypeFamily);
            Assert.AreEqual(FieldTypes.Decimal, threshold.FieldType);
            Assert.IsTrue(threshold.IsNullable);
        }

        [TestMethod]
        public void ShouldProduceDeterministicPairSummary()
        {
            var entries = FormFieldMetadataInventory.ScanTypes(new[] { typeof(SampleFormModel) });
            var pairs = FormFieldMetadataInventory.SummarizePairs(entries);

            CollectionAssert.AreEqual(
                new[] { "DateTime/DateTime", "Decimal/Decimal", "Integer/Integer", "String/Text" },
                pairs.Select(pair => pair.PairKey).ToArray());
        }
    }
}
