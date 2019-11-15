using Quartz;
using Quartz.Util;
using System.Linq;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace Topshelf.Quartz
{
    public static class SchedulerExtensions
    {
        public static async Task Configure(this IScheduler scheduler, SchedulerConfigurator schedulerConfigurator)
        {
            if (schedulerConfigurator == null)
                return;

            var log = HostLogger.Get(typeof(SchedulerConfigurator));

            if (schedulerConfigurator.JobListeners != null && schedulerConfigurator.JobListeners.Any())
            {
                foreach (var listener in schedulerConfigurator.JobListeners)
                {
                    var config = listener();
                    scheduler.ListenerManager.AddJobListener(config.Listener, config.Matchers);
                    log.Info($"[Topshelf.Quartz] Added Job Listener: {config.Listener.Name}");
                }
            }

            if (schedulerConfigurator.TriggerListeners != null && schedulerConfigurator.TriggerListeners.Any())
            {
                foreach (var listener in schedulerConfigurator.TriggerListeners)
                {
                    var config = listener();
                    scheduler.ListenerManager.AddTriggerListener(config.Listener, config.Matchers);
                    log.Info($"[Topshelf.Quartz] Added Trigger Listener: {config.Listener.Name}");
                }
            }

            if (schedulerConfigurator.ScheduleListeners != null && schedulerConfigurator.ScheduleListeners.Any())
            {
                foreach (var listener in schedulerConfigurator.ScheduleListeners)
                {
                    var scheduleListener = listener();
                    scheduler.ListenerManager.AddSchedulerListener(scheduleListener);
                    log.Info($"[Topshelf.Quartz] Added Schedule Listener: {scheduleListener.GetType()}");
                }
            }

            if (schedulerConfigurator.Calendars != null && schedulerConfigurator.Calendars.Any())
            {
                foreach (var calendar in schedulerConfigurator.Calendars)
                {
                    var cal = calendar();

                    if (cal.CalName.IsNullOrWhiteSpace() || cal.Calendar == null)
                        continue;

                    await scheduler.AddCalendar(cal.CalName, cal.Calendar, true, true);
                    log.Info($"[Topshelf.Quartz] Added Calendar: {cal.CalName}");
                }
            }
        }
    }
}