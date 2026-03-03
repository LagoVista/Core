using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Dates
{
    public class ClalendarDateTests
    {
        [Test]
        public void Should_Create_For_Today()
        {
            var today = CalendarDate.Today();
            Console.Write(today);
            Assert.That(today.ToDateTime(), Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void StartOfMonth_Should_Return_FirstDay_Canonical()
        {
            var cd = CalendarDate.StartOfMonth(2026, 3);

            Assert.That(cd.Value, Is.EqualTo("2026-03-01"));
            Assert.That(cd.ToDateTime(), Is.EqualTo(new DateTime(2026, 3, 1)));
        }

        [Test]
        public void EndOfMonth_Should_Return_LastDay_For_31DayMonth()
        {
            var cd = CalendarDate.EndOfMonth(2026, 3);

            Assert.That(cd.Value, Is.EqualTo("2026-03-31"));
            Assert.That(cd.ToDateTime(), Is.EqualTo(new DateTime(2026, 3, 31)));
        }

        [Test]
        public void EndOfMonth_Should_Return_LastDay_For_30DayMonth()
        {
            var cd = CalendarDate.EndOfMonth(2026, 4);

            Assert.That(cd.Value, Is.EqualTo("2026-04-30"));
            Assert.That(cd.ToDateTime(), Is.EqualTo(new DateTime(2026, 4, 30)));
        }

        [Test]
        public void EndOfMonth_Should_Handle_February_NonLeapYear()
        {
            var cd = CalendarDate.EndOfMonth(2023, 2);

            Assert.That(cd.Value, Is.EqualTo("2023-02-28"));
            Assert.That(cd.ToDateTime(), Is.EqualTo(new DateTime(2023, 2, 28)));
        }

        [Test]
        public void EndOfMonth_Should_Handle_February_LeapYear()
        {
            var cd = CalendarDate.EndOfMonth(2024, 2);

            Assert.That(cd.Value, Is.EqualTo("2024-02-29"));
            Assert.That(cd.ToDateTime(), Is.EqualTo(new DateTime(2024, 2, 29)));
        }

        [TestCase(0)]
        [TestCase(13)]
        [TestCase(-1)]
        public void StartOfMonth_InvalidMonth_ShouldThrow(int month)
        {
            Assert.That(() => CalendarDate.StartOfMonth(2026, month), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(13)]
        [TestCase(-1)]
        public void EndOfMonth_InvalidMonth_ShouldThrow(int month)
        {
            Assert.That(() => CalendarDate.EndOfMonth(2026, month), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void StartOfMonth_InvalidYear_ShouldThrow()
        {
            // DateTime year range starts at 1, so 0 is invalid
            Assert.That(() => CalendarDate.StartOfMonth(0, 1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void EndOfMonth_InvalidYear_ShouldThrow()
        {
            Assert.That(() => CalendarDate.EndOfMonth(0, 1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void AddDays_Positive_Should_Advance_Date()
        {
            var date = new CalendarDate("2026-03-10");

            var result = date.AddDays(5);

            Assert.That(result.Value, Is.EqualTo("2026-03-15"));
            Assert.That(result.ToDateTime(), Is.EqualTo(new DateTime(2026, 3, 15)));
        }

        [Test]
        public void AddDays_Negative_Should_Go_Backwards()
        {
            var date = new CalendarDate("2026-03-10");

            var result = date.AddDays(-10);

            Assert.That(result.Value, Is.EqualTo("2026-02-28"));
            Assert.That(result.ToDateTime(), Is.EqualTo(new DateTime(2026, 2, 28)));
        }

        [Test]
        public void AddDays_Should_Cross_Month_Boundary()
        {
            var date = new CalendarDate("2026-01-30");

            var result = date.AddDays(5);

            Assert.That(result.Value, Is.EqualTo("2026-02-04"));
        }

        [Test]
        public void AddDays_Should_Handle_LeapYear()
        {
            var date = new CalendarDate("2024-02-28");

            var result = date.AddDays(1);

            Assert.That(result.Value, Is.EqualTo("2024-02-29"));
        }

        [Test]
        public void AddDays_Should_Handle_Year_Boundary()
        {
            var date = new CalendarDate("2026-12-31");

            var result = date.AddDays(1);

            Assert.That(result.Value, Is.EqualTo("2027-01-01"));
        }

        [Test]
        public void AddDays_Zero_Should_Return_Same_Date()
        {
            var date = new CalendarDate("2026-03-10");

            var result = date.AddDays(0);

            Assert.That(result, Is.EqualTo(date));
        }

        [Test]
        public void Today_Should_Match_System_Today_Canonical()
        {
            var today = DateTime.Today;

            var cd = CalendarDate.Today();

            Assert.That(cd.Value, Is.EqualTo(today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            Assert.That(cd.ToDateTime(), Is.EqualTo(today.Date));
        }

        [Test]
        public void Today_Should_Be_Canonical_Format_Length_10()
        {
            var cd = CalendarDate.Today();

            Assert.That(cd.Value, Has.Length.EqualTo(10));
            Assert.That(cd.Value, Does.Match(@"^\d{4}-\d{2}-\d{2}$"));
        }

        [Test]
        public void Today_Should_RoundTrip_Through_ToDateTime_Without_Change()
        {
            var today = DateTime.Today;

            var cd = CalendarDate.Today();
            var roundTrip = new CalendarDate(cd.ToDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            Assert.That(roundTrip.Value, Is.EqualTo(today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            Assert.That(roundTrip, Is.EqualTo(cd));
        }

        [Test]
        public void YearMonthDay_Should_Extract_Correct_Values()
        {
            var date = new CalendarDate("2026-03-15");

            Assert.That(date.Year, Is.EqualTo(2026));
            Assert.That(date.Month, Is.EqualTo(3));
            Assert.That(date.Day, Is.EqualTo(15));
        }

        [Test]
        public void YearMonthDay_Should_Handle_Leap_Day()
        {
            var date = new CalendarDate("2024-02-29");

            Assert.That(date.Year, Is.EqualTo(2024));
            Assert.That(date.Month, Is.EqualTo(2));
            Assert.That(date.Day, Is.EqualTo(29));
        }
    }
}
