using NUnit.Framework;
using LagoVista.Core;
using System;

namespace LagoVista.Core.Tests.Dates
{
    [TestFixture]
    public class CalendarDateComparableTests
    {
        [Test]
        public void CompareTo_Self_ReturnsZero()
        {
            var date = new CalendarDate("2024-03-15");
            Assert.That(date.CompareTo(date), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_EarlierDate_ReturnsPositive()
        {
            var later = new CalendarDate("2024-03-15");
            var earlier = new CalendarDate("2024-03-10");
            Assert.That(later.CompareTo(earlier), Is.GreaterThan(0));
            Assert.That(earlier.CompareTo(later), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_DifferentMonths_ReturnsCorrectSign()
        {
            var mar = new CalendarDate("2024-03-15");
            var feb = CalendarDate.StartOfMonth(2024, 2);
            Assert.That(mar.CompareTo(feb), Is.GreaterThan(0));
            Assert.That(feb.CompareTo(mar), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_DifferentYears_ReturnsCorrectSign()
        {
            var date2024 = new CalendarDate("2024-01-01");
            var date2023 = new CalendarDate("2023-12-31");
            Assert.That(date2024.CompareTo(date2023), Is.GreaterThan(0));
            Assert.That(date2023.CompareTo(date2024), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_TodayVsFixedDate()
        {
            var today = CalendarDate.Today();
            var fixedPast = CalendarDate.Create(2020, 1, 1);
            if (today.CompareTo(fixedPast) < 0)
                Assert.Fail("Test assumes today > 2020-01-01");
            Assert.That(today.CompareTo(fixedPast), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_LeapYear()
        {
            var leap = new CalendarDate("2024-02-29");
            var nonLeap = CalendarDate.Create(2023, 2, 28);
            Assert.That(leap.CompareTo(nonLeap), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_ObjectNonCalendarDate_ThrowsArgumentException()
        {
            var date = new CalendarDate("2024-03-15");
            var obj = 123;
            Assert.That(() => date.CompareTo(obj), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CompareTo_EqualDates_ReturnsZero()
        {
            var date1 = CalendarDate.EndOfMonth(2024, 2);
            var date2 = new CalendarDate("2024-02-29");
            Assert.That(date1.CompareTo(date2), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_MinDateVsMaxDate()
        {
            var minDate = CalendarDate.Create(1, 1, 1);
            var maxDate = CalendarDate.EndOfMonth(9999, 12);
            Assert.That(minDate.CompareTo(maxDate), Is.LessThan(0));
            Assert.That(maxDate.CompareTo(minDate), Is.GreaterThan(0));
        }
    }
}

