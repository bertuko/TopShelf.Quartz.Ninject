using Quartz;
using System;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz.Extensions
{
    public static class TopshelfQuartzServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<TService> ScheduleQuartzJobWithCronSchedule<TService, TJob>(
            this ServiceConfigurator<TService> configurator, string cronSchedule, string calendarName = null)
            where TJob : IJob where TService : class
        {
            return configurator.ScheduleQuartzJobWithTrigger<TService, TJob>(TopshelfQuartzTriggerExtensions.GetCronScheduleTrigger(cronSchedule, calendarName));
        }

        public static ServiceConfigurator<TService> ScheduleQuartzJobWithSimpleSchedule<TService, TJob>(
            this ServiceConfigurator<TService> configurator, Action<SimpleScheduleBuilder> action, string calendarName = null)
            where TJob : IJob where TService : class
        {
            return configurator.ScheduleQuartzJobWithTrigger<TService, TJob>(TopshelfQuartzTriggerExtensions.GetSimpleScheduleTrigger(action, calendarName));
        }

        public static ServiceConfigurator<TService> ScheduleQuartzJobWithTrigger<TService, TJob>(
            this ServiceConfigurator<TService> configurator, Func<ITrigger> trigger)
            where TJob : IJob where TService : class
        {
            return configurator.ScheduleQuartzJob(q =>
            {
                q.WithJob(() => JobBuilder.Create<TJob>().Build()).AddTrigger(trigger);
            });
        }
    }
}