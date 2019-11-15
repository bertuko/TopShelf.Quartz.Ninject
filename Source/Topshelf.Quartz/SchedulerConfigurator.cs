using Quartz;
using System;
using System.Collections.Generic;

namespace Topshelf.Quartz
{
    public class SchedulerConfigurator
    {
        public IList<Func<QuartzJobListenerConfig>> JobListeners { get; }
        public IList<Func<QuartzTriggerListenerConfig>> TriggerListeners { get; }
        public IList<Func<ISchedulerListener>> ScheduleListeners { get; }
        public IList<Func<QuartzCalendarConfig>> Calendars { get; }

        public SchedulerConfigurator()
        {
            TriggerListeners = new List<Func<QuartzTriggerListenerConfig>>();
            JobListeners = new List<Func<QuartzJobListenerConfig>>();
            ScheduleListeners = new List<Func<ISchedulerListener>>();
            Calendars = new List<Func<QuartzCalendarConfig>>();
        }

        public SchedulerConfigurator WithJobListener(Func<QuartzJobListenerConfig> jobListener)
        {
            JobListeners.Add(jobListener);
            return this;
        }

        public SchedulerConfigurator WithTriggerListener(Func<QuartzTriggerListenerConfig> triggerListener)
        {
            TriggerListeners.Add(triggerListener);
            return this;
        }

        public SchedulerConfigurator WithScheduleListener(Func<ISchedulerListener> scheduleListener)
        {
            ScheduleListeners.Add(scheduleListener);
            return this;
        }

        public SchedulerConfigurator WithCalendar(Func<QuartzCalendarConfig> scheduleListener)
        {
            Calendars.Add(scheduleListener);
            return this;
        }
    }
}