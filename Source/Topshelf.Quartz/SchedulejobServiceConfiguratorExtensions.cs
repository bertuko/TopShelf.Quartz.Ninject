using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topshelf.Logging;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz
{
    public static class ScheduleJobServiceConfiguratorExtensions
    {
        private static readonly Func<Task<IScheduler>> DefaultSchedulerFactory = async () =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                return await schedulerFactory.GetScheduler();
            };

        private static Func<Task<IScheduler>> _customSchedulerFactory;
        private static IScheduler _scheduler;
        private static IJobFactory _jobFactory;
        private static Func<SchedulerConfigurator> _schedulerConfigurator;

        private static async Task<IScheduler> GetScheduler()
        {
            if (_scheduler != null)
                return _scheduler;

            var schedulerFactory = _customSchedulerFactory ?? DefaultSchedulerFactory;
            _scheduler = await schedulerFactory();

            if (_jobFactory != null)
                _scheduler.JobFactory = _jobFactory;

            if (_schedulerConfigurator != null)
                await _scheduler.Configure(_schedulerConfigurator());

            return _scheduler;
        }

        public static ServiceConfigurator<T> UsingSchedulerFactory<T>(this ServiceConfigurator<T> configurator,
            Func<Task<IScheduler>> schedulerFactory) where T : class
        {
            _customSchedulerFactory = schedulerFactory;
            return configurator;
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(
            this ServiceConfigurator<T> configurator, Func<TJobFactory> jobFactory)
            where T : class where TJobFactory : IJobFactory
        {
            _jobFactory = jobFactory();
            return configurator;
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(
            this ServiceConfigurator<T> configurator) where T : class where TJobFactory : IJobFactory, new()
        {
            return UsingQuartzJobFactory(configurator, () => new TJobFactory());
        }

        public static ServiceConfigurator<T> ScheduleQuartzJob<T>(this ServiceConfigurator<T> configurator,
            Action<JobConfigurator> jobConfigurator, bool replaceJob = false) where T : class
        {
            ConfigureJob(configurator, jobConfigurator, replaceJob);
            return configurator;
        }

        public static ServiceConfigurator<T> ConfigureQuartzScheduler<T>(this ServiceConfigurator<T> configurator,
            Func<SchedulerConfigurator> schedulerConfigurator) where T : class
        {
            _schedulerConfigurator = schedulerConfigurator;
            return configurator;
        }

        private static void ConfigureJob<T>(ServiceConfigurator<T> configurator, Action<JobConfigurator> jobConfigurator, bool replaceJob = false) where T : class
        {
            var log = HostLogger.Get(typeof(ScheduleJobServiceConfiguratorExtensions));

            var jobConfig = new JobConfigurator();
            jobConfigurator(jobConfig);

            if ((jobConfig.JobEnabled == null || jobConfig.JobEnabled()) && jobConfig.Job != null && jobConfig.Triggers != null)
            {
                var jobDetail = jobConfig.Job();
                var jobTriggers = jobConfig.Triggers.Select(triggerFactory => triggerFactory()).Where(trigger => trigger != null).ToHashSet()
#if NET452
                    .ToArray()
#endif
                    ;

                async Task BeforeStartingServiceFunc()
                {
                    log.Debug("[Topshelf.Quartz] Scheduler starting up...");

                    var scheduler = await GetScheduler();
                    if (scheduler != null && jobDetail != null && jobTriggers.Any())
                    {
                        await scheduler.ScheduleJob(jobDetail, jobTriggers, replaceJob);
                        log.Info($"[Topshelf.Quartz] Scheduled Job: {jobDetail.Key} of Type: {jobDetail.JobType.Name}");

                        foreach (var trigger in jobTriggers)
                        {
                            log.Info($"[Topshelf.Quartz] Job Schedule: {trigger} - Next Fire Time (local): {trigger.GetNextFireTimeUtc()?.ToLocalTime().ToString() ?? "none"}");
                        }

                        await scheduler.Start();
                        log.Info("[Topshelf.Quartz] Scheduler started...");
                    }
                }

                async Task BeforeStoppingServiceFunc()
                {
                    log.Debug("[Topshelf.Quartz] Scheduler shutting down...");
                    var scheduler = await GetScheduler();
                    if (scheduler != null)
                        if (!scheduler.IsShutdown)
                            await scheduler.Shutdown();
                    log.Info("[Topshelf.Quartz] Scheduler shut down...");
                }

                configurator.BeforeStartingService(async () => await BeforeStartingServiceFunc());
                configurator.BeforeStoppingService(async () => await BeforeStoppingServiceFunc());
            }
        }

        private static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
        {
            return new HashSet<T>(collection);
        }
    }
}
