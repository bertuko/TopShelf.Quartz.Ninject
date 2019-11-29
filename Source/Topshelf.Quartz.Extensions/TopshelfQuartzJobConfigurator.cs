using Quartz;
using System;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz.Extensions
{
    public class TopshelfQuartzJobConfigurator<TService> where TService : class
    {
        private readonly ServiceConfigurator<TService> _configurator;

        public TopshelfQuartzJobConfigurator(ServiceConfigurator<TService> configurator)
        {
            _configurator = configurator;
        }

        public TopshelfQuartzJobConfigurator<TService> WithCronSchedule<TJob>(string cronSchedule, string calendarName = null)
            where TJob : IJob
        {
            _configurator.ScheduleQuartzJobWithCronSchedule<TService, TJob>(cronSchedule, calendarName);
            return this;
        }

        public TopshelfQuartzJobConfigurator<TService> WithSimpleSchedule<TJob>(Action<SimpleScheduleBuilder> action, string calendarName = null)
            where TJob : IJob
        {
            _configurator.ScheduleQuartzJobWithSimpleSchedule<TService, TJob>(action, calendarName);
            return this;
        }

        public TopshelfQuartzJobConfigurator<TService> WithTrigger<TJob>(Func<ITrigger> trigger)
            where TJob : IJob
        {
            _configurator.ScheduleQuartzJobWithTrigger<TService, TJob>(trigger);
            return this;
        }
    }
}