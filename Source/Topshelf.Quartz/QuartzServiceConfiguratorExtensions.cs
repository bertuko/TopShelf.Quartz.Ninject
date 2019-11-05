using Quartz;
using System;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz
{
    public static class QuartzServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<TService> ScheduleQuartzJobWithCronSchedule<TService, TJob>(
            this ServiceConfigurator<TService> configurator, string cronSchedule)
            where TJob : IJob where TService : class
        {
            return configurator.ScheduleQuartzJobWithTrigger<TService, TJob>(GetCronScheduleTrigger<TJob>(cronSchedule));
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

        private static Func<ITrigger> GetCronScheduleTrigger<TJob>(string cronSchedule) where TJob : IJob
        {
            // https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
            return () => TriggerBuilder.Create()
                    .WithCronSchedule(cronSchedule)
                    .WithIdentity(typeof(TJob).Name)
                    .Build();
        }
    }
}