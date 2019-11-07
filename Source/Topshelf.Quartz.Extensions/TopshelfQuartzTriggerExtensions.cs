using Quartz;
using System;

namespace Topshelf.Quartz.Extensions
{
    public static class TopshelfQuartzTriggerExtensions
    {
        public static Func<ITrigger> GetCronScheduleTrigger(string cronSchedule, string calendarName = null)
        {
            return () => TriggerBuilder.Create().WithCronSchedule(cronSchedule).ModifiedByCalendarSafe(calendarName).Build();
        }

        public static Func<ITrigger> GetSimpleScheduleTrigger(Action<SimpleScheduleBuilder> action, string calendarName = null)
        {
            return () => TriggerBuilder.Create().WithSimpleSchedule(action).ModifiedByCalendarSafe(calendarName).Build();
        }

        private static TriggerBuilder ModifiedByCalendarSafe(this TriggerBuilder triggerBuilder, string calendarName)
        {
            if (calendarName != null)
                triggerBuilder.ModifiedByCalendar(calendarName);
            return triggerBuilder;
        }
    }
}