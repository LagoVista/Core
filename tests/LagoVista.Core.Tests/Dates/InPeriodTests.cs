using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Dates
{
    [TestFixture]
    public sealed class InPeriodTests
    {
        [Test]
        public void CalendarDate_InPeriod_InclusiveStartAndEnd()
        {
            var start = new CalendarDate("2026/01/01"); // legacy accepted
            var end = new CalendarDate("2026-12-31");   // new accepted

            Assert.That(new CalendarDate("2026-01-01").InPeriod(start, end), Is.True);
            Assert.That(new CalendarDate("2026/12/31").InPeriod(start, end), Is.True);

            Assert.That(new CalendarDate("2025-12-31").InPeriod(start, end), Is.False);
            Assert.That(new CalendarDate("2027-01-01").InPeriod(start, end), Is.False);
        }

        [Test]
        public void UtcTimestamp_InPeriod_IncludesAnyTimeOnEndDay()
        {
            var start = new CalendarDate("2026-01-01");
            var end = new CalendarDate("2026-12-31");

            Assert.That(new UtcTimestamp("2026-12-31T00:00:00.000Z").InPeriod(start, end), Is.True);
            Assert.That(new UtcTimestamp("2026-12-31T14:24:00.000Z").InPeriod(start, end), Is.True);
            Assert.That(new UtcTimestamp("2026-12-31T23:59:59.999Z").InPeriod(start, end), Is.True);
        }

        [Test]
        public void UtcTimestamp_InPeriod_ExcludesEndExclusiveBoundary()
        {
            var start = new CalendarDate("2026-01-01");
            var end = new CalendarDate("2026-12-31");

            Assert.That(new UtcTimestamp("2027-01-01T00:00:00.000Z").InPeriod(start, end), Is.False);
        }

        [Test]
        public void UtcTimestamp_InPeriod_ExcludesBeforeStart()
        {
            var start = new CalendarDate("2026-01-01");
            var end = new CalendarDate("2026-12-31");

            Assert.That(new UtcTimestamp("2025-12-31T23:59:59.999Z").InPeriod(start, end), Is.False);
        }

        [Test]
        public void InPeriod_ThrowsWhenEndBeforeStart()
        {
            var start = new CalendarDate("2026-02-01");
            var end = new CalendarDate("2026-01-31");

            Assert.That(() => new CalendarDate("2026-01-31").InPeriod(start, end), Throws.ArgumentException);
            Assert.That(() => new UtcTimestamp("2026-01-31T12:00:00.000Z").InPeriod(start, end), Throws.ArgumentException);
        }
    }
}