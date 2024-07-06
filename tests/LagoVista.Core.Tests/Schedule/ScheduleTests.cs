using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Schedule
{
    [TestClass]
    public class ScheduleTests
    {

        ScheduleManager _scheduleMgr;


        [TestInitialize]
        public void Init()
        {
            _scheduleMgr = new ScheduleManager();
        }

        public DateTime GetReferenceDate(int year = 2024, int month = 7, int day = 05, int hours = 13, int minutes = 30, int seconds = 0)
        {
            return new DateTime(year, month, day, hours, minutes, seconds);
        }

        private Core.Models.Schedule CreateSchedule(Core.Models.ScheduleTypes scheduleType, Core.Models.WeekNumbers? weekNumber = null, Core.Models.WeekDays? dayOfWeek = null, string startTime = null)
        {
            var schedule = new LagoVista.Core.Models.Schedule()
            {
                ScheduleType = Core.Models.EntityHeader<Core.Models.ScheduleTypes>.Create(scheduleType),
            };

            if(weekNumber.HasValue)
            {
                schedule.WeekNumber = Core.Models.EntityHeader<Core.Models.WeekNumbers>.Create(weekNumber.Value);
            }

            if (dayOfWeek.HasValue)
            {
                schedule.DayOfWeek = Core.Models.EntityHeader<Core.Models.WeekDays>.Create(dayOfWeek.Value);
            }

            if(!String.IsNullOrEmpty(startTime))
            {
                schedule.StartTime = startTime;
            }

            return schedule;
        }

        private void Validate(DateTime date, int year, int month, int day, int hour, int minute)
        {
            Assert.AreEqual(year, date.Year, "Incorrect Year");
            Assert.AreEqual(month, date.Month, "Incorrect Month");
            Assert.AreEqual(day, date.Day, "Incorrect Day");
            Assert.AreEqual(hour, date.Hour, "Incorrect Hour");
            Assert.AreEqual(minute, date.Minute, "Incorrect Minute");
        }

        [TestMethod]
        public void Daily_Schedule_After_Now_Today()
        {
            var schedule = CreateSchedule(ScheduleTypes.Daily, dayOfWeek: WeekDays.Friday, startTime: "1330");
            var refDate = GetReferenceDate(hours: 12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 5, 13, 30);
        }

        [TestMethod]
        public void Daily_Schedule_Before_Now_Tomorrow()
        {
            var schedule = CreateSchedule(ScheduleTypes.Daily, dayOfWeek: WeekDays.Friday, startTime: "0900");
            var refDate = GetReferenceDate(hours: 12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 6, 09, 00);
        }


        [TestMethod]
        public void BiWeekly_Schedule_Monday_On_Friday()
        {
            var schedule = CreateSchedule(ScheduleTypes.Biweekly, dayOfWeek: WeekDays.Monday, startTime: "1330");
            var refDate = GetReferenceDate();
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 8, 13, 30);
        }

        [TestMethod]
        public void BiWeekly_Friday_On_Monday()
        {
            var schedule = CreateSchedule(ScheduleTypes.Biweekly, dayOfWeek: WeekDays.Friday, startTime: "1330");
            var refDate = GetReferenceDate(day:1);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 5, 13, 30);
        }

        [TestMethod]
        public void BiWeekly_Monday_On_Monday_Schedule_After_Current_Next_Week()
        {
            var schedule = CreateSchedule(ScheduleTypes.Biweekly, dayOfWeek: WeekDays.Friday, startTime: "1330");
            var refDate = GetReferenceDate(day: 5, hours: 12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 5, 13, 30);
        }

        [TestMethod]
        public void BiWeekly_Monday_On_Monday_Schedule_Before_Current_This_Week()
        {
            var schedule = CreateSchedule(ScheduleTypes.Biweekly, dayOfWeek: WeekDays.Friday, startTime: "0900");
            var refDate = GetReferenceDate(day: 5, hours: 12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 12, 09, 00);
        }

        [TestMethod]
        public void Weekly_Schedule_Monday_On_Friday()
        {
            var schedule = CreateSchedule(ScheduleTypes.Weekly, startTime: "1330");
            schedule.Monday = true;
            var refDate = GetReferenceDate();
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 8, 13, 30);
        }

        [TestMethod]
        public void Weekly_Schedule_SameDay_Before_Schedule()
        {
            var schedule = CreateSchedule(ScheduleTypes.Weekly, startTime: "0915");
            schedule.Friday = true;
            var refDate = GetReferenceDate(hours: 12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 12, 09, 15);
        }

        [TestMethod]
        public void Weekly_Schedule_SameDay_After_Schedule()
        {
            var schedule = CreateSchedule(ScheduleTypes.Weekly, startTime: "1330");
            schedule.Friday = true;
            var refDate = GetReferenceDate(hours:12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 5, 13, 30);
        }

        [TestMethod]
        public void Weekly_Schedule_Friday_On_Monday()
        {
            var schedule = CreateSchedule(ScheduleTypes.Weekly, startTime: "1330");
            schedule.Friday = true;
            var refDate = GetReferenceDate(day:8);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 12, 13, 30);
        }

        [TestMethod]
        public void Schedule_First_Day_Of_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.FirstDayOfMonth, startTime:"1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate());
            Console.WriteLine(response);
            Validate(response, 2024, 08, 1, 13, 30);
        }

        [TestMethod]
        public void Schedule_First_Day_Of_Month_OnFirst_BeforeSchedule()
        {
            var schedule = CreateSchedule(ScheduleTypes.FirstDayOfMonth, startTime: "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate(day: 1, hours: 12));
            Console.WriteLine(response);
            Validate(response, 2024, 07, 1, 13, 30);
        }


        [TestMethod]
        public void Schedule_First_Day_Of_Month_OnFirst_AfterSchedule()
        {
            var schedule = CreateSchedule(ScheduleTypes.FirstDayOfMonth, startTime: "0915");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate(day:1, hours:12));
            Console.WriteLine(response);
            Validate(response, 2024, 08, 1, 09, 15);
        }


        [TestMethod]
        public void Schedule_Last_Day_Of_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.LaststDayOfMonth, startTime: "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate());
            Console.WriteLine(response);
            Validate(response, 2024, 07, 31, 13, 30);
        }

        [TestMethod]
        public void Schedule_Last_Day_Of_Month_OnLast_BeforeSchedule()
        {
            var schedule = CreateSchedule(ScheduleTypes.LaststDayOfMonth, startTime: "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate(day: 31, hours: 12));
            Console.WriteLine(response);
            Validate(response, 2024, 07, 31, 13, 30);
        }


        [TestMethod]
        public void Schedule_Last_Day_Of_Month_OnLast_AfterSchedule()
        {
            var schedule = CreateSchedule(ScheduleTypes.LaststDayOfMonth, startTime: "0915");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate(day: 31, hours: 12));
            Console.WriteLine(response);
            Validate(response, 2024, 08, 31, 09, 15);
        }


        [TestMethod]
        public void Schedule_ThirdThursday_On_First_Friday_This_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.Monthly, WeekNumbers.ThirdWeek, WeekDays.Thursday, "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate());
            Console.WriteLine(response);
            Validate(response, 2024, 07, 18, 13, 30);
        }

        [TestMethod]
        public void When_Schedule_Is_Same_Day_But_Now_After_ScheduleDate_In_Day_Schedule_This_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.Monthly, WeekNumbers.FirstWeek, WeekDays.Friday, "0930");
            var refDate = GetReferenceDate(hours:12);            
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 08, 2, 9, 30);
        }

        [TestMethod]
        public void When_Schedule_Is_Same_Day_But_Now_Earlier_In_Day_Schedule_This_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.Monthly, WeekNumbers.FirstWeek, WeekDays.Friday, "1330");
            var refDate = GetReferenceDate(hours: 12);
            var response = _scheduleMgr.GetInitialSchedule(schedule, refDate);
            Console.WriteLine(response);
            Validate(response, 2024, 07, 5, 13, 30);
        }


        [TestMethod]
        public void Schedule_First_Monday_On_First_Friday_Schedule_Next_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.Monthly, WeekNumbers.FirstWeek, WeekDays.Monday, "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate());
            Console.WriteLine(response);
            Validate(response, 2024, 8, 5, 13, 30);
        }

        [TestMethod]
        public void Schedule_First_Sunday_On_First_Friday_Schedule_Next_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.Monthly, WeekNumbers.FirstWeek, WeekDays.Sunday, "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate());
            Console.WriteLine(response);
            Validate(response, 2024, 8, 4, 13, 30);
        }

        [TestMethod]
        public void Schedule_First_Saturday_On_First_Friday_Schedule_This_Month()
        {
            var schedule = CreateSchedule(ScheduleTypes.Monthly, WeekNumbers.FirstWeek, WeekDays.Saturday, "1330");
            var response = _scheduleMgr.GetInitialSchedule(schedule, GetReferenceDate());
            Console.WriteLine(response);
            Validate(response, 2024, 7, 6, 13, 30);
        }

    }
}
