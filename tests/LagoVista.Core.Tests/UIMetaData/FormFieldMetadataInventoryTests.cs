using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Tests.UIMetaData
{
    [TestClass]
    public class FormFieldMetadataInventoryTests
    {
        private class SampleFormModel
        {
            [FormField(FieldType: FieldTypes.Text)]
            public string Name { get; set; }

            [FormField(FieldType: FieldTypes.Integer)]
            public int RetryCount { get; set; }

            [FormField(FieldType: FieldTypes.Decimal)]
            public decimal? Threshold { get; set; }

            [FormField(FieldType: FieldTypes.DateTime)]
            public DateTime CreatedUtc { get; set; }

            [FormField(FieldType: FieldTypes.ChildList)]
            public List<string> Tags { get; set; }

            [FormField(FieldType: FieldTypes.ChildList)]
            public int[] Samples { get; set; }

            public string NotAFormField { get; set; }
        }

        [TestMethod]
        public void ShouldInventoryFormFieldClrTypeAndFieldType()
        {
            var entries = FormFieldMetadataInventory.ScanTypes(new[] { typeof(SampleFormModel) });

            Assert.AreEqual(6, entries.Count);

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
        public void ShouldNormalizeCollectionFamiliesWithoutLosingExactClrType()
        {
            var entries = FormFieldMetadataInventory.ScanTypes(new[] { typeof(SampleFormModel) });

            var tags = entries.Single(entry => entry.PropertyName == nameof(SampleFormModel.Tags));
            Assert.AreEqual("List", tags.ClrTypeFamily);
            StringAssert.Contains(tags.ClrType, "List");
            StringAssert.Contains(tags.ClrType, "System.String");

            var samples = entries.Single(entry => entry.PropertyName == nameof(SampleFormModel.Samples));
            Assert.AreEqual("Array", samples.ClrTypeFamily);
            Assert.AreEqual("System.Int32[]", samples.ClrType);
        }

        [TestMethod]
        public void ShouldProduceDeterministicPairSummary()
        {
            var entries = FormFieldMetadataInventory.ScanTypes(new[] { typeof(SampleFormModel) });
            var pairs = FormFieldMetadataInventory.SummarizePairs(entries);

            CollectionAssert.AreEqual(
                new[] { "Array/ChildList", "DateTime/DateTime", "Decimal/Decimal", "Integer/Integer", "List/ChildList", "String/Text" },
                pairs.Select(pair => pair.PairKey).ToArray());
        }
    }
}
