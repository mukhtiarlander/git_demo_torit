using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduleWidget.Enums;
using ScheduleWidget.ScheduledEvents;

namespace RDN.Tests.RDN.Library.Calendar
{
    [TestClass]
    public class Calendar
    {
        [TestMethod]
        public void TestMontlyCalendar()
        {
            var aEvent = new Event()
            {
                ID = 1,
                Title = "Critical Mass",
                FrequencyTypeOptions = FrequencyTypeEnum.Monthly,
                MonthlyIntervalOptions = MonthlyIntervalEnum.First,
                DaysOfWeekOptions = DayOfWeekEnum.Fri
            }; var schedule = new Schedule(aEvent);
            var range = new DateRange()
            {
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddYears(1)
            };
            var occurrences = schedule.Occurrences(range);

        }
    }
}
