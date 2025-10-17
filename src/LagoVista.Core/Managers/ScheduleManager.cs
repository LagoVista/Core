// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8b259e4ee7ab7ba51bba1863f2e26ae40c18534a934661925956ffa9addb89ad
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Diagnostics;

namespace LagoVista.Core.Managers
{
    public class ScheduleManager
    {
        static int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }

        private bool CanScheduleForDate(Schedule schedule, DateTime now)
        {
            if (String.IsNullOrEmpty(schedule.StartTime))
                return false;

            var scheduleTime = schedule.StartTime.ToTimeSpan();

            return scheduleTime.Value.Hours > now.TimeOfDay.Hours ||
                (scheduleTime.Value.Hours == now.TimeOfDay.Hours &&
                scheduleTime.Value.Minutes > now.TimeOfDay.Minutes);
        }


        private DateTime ScheduleWeekOfMonth(Schedule schedule, DateTime now)
        {
            var currentWeekOfMonth = GetWeekNumberOfMonth(now) - 1; /* -1 since the value here is 1 index, we want 0 index for compares */
            var scheduleWeekOfMonth = (int)schedule.WeekNumber.Value;

            var currentDOW = (int)now.DayOfWeek;
            var dowIndex = (int)schedule.DayOfWeek.Value;
            var sameDay = currentDOW == dowIndex;
            var pastDayInWeek = currentDOW > dowIndex;

            if (currentWeekOfMonth == scheduleWeekOfMonth)
            {
                // We are in the same week as the schedule wants
                if (sameDay)
                {
                    var calculatedSchedule = new DateTime(now.Year, now.Month, now.Day);
                   

                    if (!CanScheduleForDate(schedule, now))
                    {
                        calculatedSchedule = calculatedSchedule.AddDays(28);
                    }

                    calculatedSchedule = ConditionallyAddTime(schedule, calculatedSchedule);

                    return calculatedSchedule;
                }
                else
                {
                    var calculatedSchedule = new DateTime(now.Year, now.Month, now.Day);
                    if (pastDayInWeek)
                    {
                        calculatedSchedule = calculatedSchedule.AddMonths(1);
                    }

                    calculatedSchedule = ConditionallyAddTime(schedule, calculatedSchedule);
                    calculatedSchedule = calculatedSchedule.AddDays(dowIndex - (int)calculatedSchedule.DayOfWeek); 
                    return calculatedSchedule;
                }
            }
            else if (scheduleWeekOfMonth > currentWeekOfMonth)
            {
                /* haven't got there this month, schedule this month */
                var calculatedSchedule = new DateTime(now.Year, now.Month, scheduleWeekOfMonth * 7);
                calculatedSchedule = ConditionallyAddTime(schedule, calculatedSchedule);
                calculatedSchedule = calculatedSchedule.AddDays(dowIndex - (int)calculatedSchedule.DayOfWeek);
                return calculatedSchedule;
            }
            else if (scheduleWeekOfMonth < currentWeekOfMonth)
            {
                var calculatedSchedule = new DateTime(now.Year, now.Month, scheduleWeekOfMonth * 7);
                calculatedSchedule = calculatedSchedule.AddMonths(1);
                calculatedSchedule = ConditionallyAddTime(schedule, calculatedSchedule);

                calculatedSchedule = calculatedSchedule.AddDays(dowIndex - (int)calculatedSchedule.DayOfWeek);
                return calculatedSchedule;
                /* missed it this month, schedule for next month */
            }

            throw new Exception("should never get here, was not sameday, pastTimeInDay or beforeDayInWeek");
        }

        private DateTime ScheduleBiWeekly(Schedule schedule, DateTime now)
        {
            var currentDOW = (int)now.DayOfWeek;
            var dowIndex = (int)schedule.DayOfWeek.Value;
            var sameDay = currentDOW == dowIndex;
            var pastDayInWeek = currentDOW > dowIndex;
            if (sameDay)
            {
                var calculatedSchedule = new DateTime(now.Year, now.Month, now.Day);


                if (!CanScheduleForDate(schedule, now))
                {
                    calculatedSchedule = calculatedSchedule.AddDays(7);
                }

                calculatedSchedule = ConditionallyAddTime(schedule, calculatedSchedule);

                return calculatedSchedule;
            }
            else
            {
                var calculatedSchedule = new DateTime(now.Year, now.Month, now.Day);
                if (pastDayInWeek)
                {
                    calculatedSchedule = calculatedSchedule.AddDays(7);
                }

                calculatedSchedule = ConditionallyAddTime(schedule, calculatedSchedule);
                calculatedSchedule = calculatedSchedule.AddDays(dowIndex - (int)calculatedSchedule.DayOfWeek);
                return calculatedSchedule;
            }
        }

        private DateTime ScheduleWeekly(Schedule schedule, DateTime now)
        {
            var currentDOW = (int)now.DayOfWeek;
            
            var scheduledDays = new bool[7];
            scheduledDays[0] = schedule.Sunday;
            scheduledDays[1] = schedule.Monday;
            scheduledDays[2] = schedule.Tuesday;
            scheduledDays[3] = schedule.Wednesday;
            scheduledDays[4] = schedule.Thursday;
            scheduledDays[5] = schedule.Friday;
            scheduledDays[6] = schedule.Saturday;

            var schedulded = new DateTime(now.Year, now.Month, now.Day);

            for (int idx = 0; idx < 7; ++idx)
            {
                if(currentDOW == idx && scheduledDays[idx] && CanScheduleForDate(schedule, now))
                {                
                    return ConditionallyAddTime(schedule, schedulded);
                }
            }

            int potentialDowIndex = currentDOW + 1;
            for (int idx = 0; idx < 7; ++idx)
            {
                schedulded = schedulded.AddDays(1);

                if (potentialDowIndex == 7)
                    potentialDowIndex = 0;
                if(scheduledDays[potentialDowIndex])
                {
                    return ConditionallyAddTime(schedule, schedulded);
                }

                potentialDowIndex++;
            }

            return DateTime.MinValue;
        }

        private DateTime ConditionallyAddTime(Schedule schedule, DateTime dateTime)
        {
            if (String.IsNullOrEmpty(schedule.StartTime))
                return dateTime;

            var scheduleTime = schedule.StartTime.ToTimeSpan();
            return dateTime.Add(scheduleTime.Value);
        }

        public DateTime GetInitialSchedule(Schedule schedule, DateTime? now = null)
        {
            if (now == null)
                now = DateTime.UtcNow;

            var currentDOW = now.Value.DayOfWeek;

            switch (schedule.ScheduleType.Value)
            {
                case ScheduleTypes.Monthly: return ScheduleWeekOfMonth(schedule, now.Value);
                case ScheduleTypes.Biweekly: return ScheduleBiWeekly(schedule, now.Value);
                case ScheduleTypes.Weekly: return ScheduleWeekly(schedule, now.Value);
                case ScheduleTypes.Daily:
                    var today = new DateTime(now.Value.Year, now.Value.Month, now.Value.Day);
                    today = ConditionallyAddTime(schedule, today);
                    if (!CanScheduleForDate(schedule, now.Value))
                        today = today.AddDays(1);
                    return today;

                case ScheduleTypes.FirstDayOfMonth:
                    if(now.Value.Day == 1)
                    {
                        var calculated = new DateTime(now.Value.Year, now.Value.Month, 1);
                        if(!CanScheduleForDate(schedule, now.Value))
                            calculated = calculated.AddMonths(1);
                        calculated = ConditionallyAddTime(schedule, calculated);
                        return calculated;
                    }
                    else
                    {
                        var calculated = new DateTime(now.Value.Year, now.Value.Month, 1);
                        calculated = calculated.AddMonths(1);
                        calculated = ConditionallyAddTime(schedule, calculated);
                        return calculated;
                    }
                case ScheduleTypes.LaststDayOfMonth:
                    var eom = new DateTime(now.Value.Year, now.Value.Month, 1);
                    eom = eom.AddMonths(1).AddDays(-1);

                    if(eom.Year == now.Value.Year && eom.Month == now.Value.Month && eom.Day == now.Value.Day)
                    {
                        if(CanScheduleForDate(schedule, now.Value))
                        {
                            return ConditionallyAddTime(schedule, eom);
                        }
                        else
                        {
                            eom = eom.AddDays(1).AddMonths(1).AddDays(-1);

                            return ConditionallyAddTime(schedule, eom);
                        }
                    }
                    else
                    {
                        return ConditionallyAddTime(schedule, eom);
                    }
            }

            return DateTime.Now;
        }
    }
}
