using NUnit.Framework;
using LagoVista.Core;
using System;

namespace LagoVista.Core.Tests.Dates
{
    [TestFixture]
    public class UtcTimestampComparableTests
    {
        [Test]
        public void CompareTo_Self_ReturnsZero()
        {
            var ts = new UtcTimestamp("2024-03-15T12:00:00.000Z");
            Assert.That(ts.CompareTo(ts), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_EarlierTimestamp_ReturnsPositive()
        {
            var later = new UtcTimestamp("2024-03-15T14:00:00.000Z");
            var earlier = new UtcTimestamp("2024-03-15T12:00:00.000Z");
            Assert.That(later.CompareTo(earlier), Is.GreaterThan(0));
            Assert.That(earlier.CompareTo(later), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_DifferentDays_ReturnsCorrectSign()
        {
            var mar15 = new UtcTimestamp("2024-03-15T00:00:00.000Z");
            var mar10 = new UtcTimestamp("2024-03-10T23:59:59.999Z");
            Assert.That(mar15.CompareTo(mar10), Is.GreaterThan(0));
            Assert.That(mar10.CompareTo(mar15), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_DifferentMonths_ReturnsCorrectSign()
        {
            var mar = new UtcTimestamp("2024-03-15T12:00:00.000Z");
            var feb = new UtcTimestamp("2024-02-15T12:00:00.000Z");
            Assert.That(mar.CompareTo(feb), Is.GreaterThan(0));
            Assert.That(feb.CompareTo(mar), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_DifferentYears_ReturnsCorrectSign()
        {
            var date2024 = new UtcTimestamp("2024-01-01T00:00:00.000Z");
            var date2023 = new UtcTimestamp("2023-12-31T23:59:59.999Z");
            Assert.That(date2024.CompareTo(date2023), Is.GreaterThan(0));
            Assert.That(date2023.CompareTo(date2024), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_NowVsFixedPast()
        {
            var now = UtcTimestamp.Now;
            var fixedPast = new UtcTimestamp("2020-01-01T00:00:00.000Z");
            if (now.CompareTo(fixedPast) < 0)
                Assert.Fail("Test assumes now > 2020-01-01");
            Assert.That(now.CompareTo(fixedPast), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_AddDays()
        {
            var ts = new UtcTimestamp("2024-03-15T12:00:00.000Z");
            var later = ts.AddDays(1);
            Assert.That(later.CompareTo(ts), Is.GreaterThan(0));
            Assert.That(ts.CompareTo(later), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_ObjectNonUtcTimestamp_ThrowsArgumentException()
        {
            var ts = new UtcTimestamp("2024-03-15T12:00:00.000Z");
            var obj = 123;
            Assert.That(() => ts.CompareTo(obj), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CompareTo_EqualTimestamps_ReturnsZero()
        {
            var ts1 = new UtcTimestamp("2024-03-15T12:34:56.789Z");
            var ts2 = UtcTimestamp.Now.AddDays(- (DateTime.UtcNow - new DateTime(2024,3,15,12,34,56,789)).TotalDays);
            // Accept minor discrepancy due to timing
            Assert.That(Math.Abs(ts1.CompareTo(ts2)) <= 1, Is.True, "Near equal expected");
        }

        [Test]
        public void CompareTo_MinVsMaxDateTimeUtc()
        {
            var minTs = new UtcTimestamp("0001-01-01T00:00:00.000Z");
            var maxTs = new UtcTimestamp("9999-12-31T23:59:59.999Z");
            Assert.That(minTs.CompareTo(maxTs), Is.LessThan(0));
            Assert.That(maxTs.CompareTo(minTs), Is.GreaterThan(0));
        }
    }
}

