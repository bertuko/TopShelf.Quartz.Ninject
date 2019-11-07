using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, ICalendar> Calendars = new ConcurrentDictionary<string, ICalendar>();

        public static Func<Task<IScheduler>> SchedulerFactory
        {
            get => _customSchedulerFactory ?? DefaultSchedulerFactory;
            set => _customSchedulerFactory = value;
        }

        private static async Task<IScheduler> GetScheduler()
        {
            var scheduler = await SchedulerFactory();

            if (_jobFactory != null)
                scheduler.JobFactory = _jobFactory;

            await scheduler.AddCalendars();
            return scheduler;
        }

        public static ServiceConfigurator<T> AddCalendar<T>(this ServiceConfigurator<T> configurator, string calendarName, ICalendar calendar) where T : class
        {
            Calendars.AddOrUpdate(calendarName, calendar, (k, v) => calendar);
            return configurator;
        }

        private static async Task AddCalendars(this IScheduler scheduler)
        {
            if (Calendars != null && Calendars.Any())
                foreach (var calendar in Calendars)
                {
                    await scheduler.AddCalendar(calendar.Key, calendar.Value, true, true);
                }
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(this ServiceConfigurator<T> configurator, Func<TJobFactory> jobFactory)
            where T : class
            where TJobFactory : IJobFactory
        {
            _jobFactory = jobFactory();
            return configurator;
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(this ServiceConfigurator<T> configurator) where T : class where TJobFactory : IJobFactory, new()
        {
            return UsingQuartzJobFactory(configurator, () => new TJobFactory());
        }

        public static ServiceConfigurator<T> ScheduleQuartzJob<T>(this ServiceConfigurator<T> configurator, Action<QuartzConfigurator> jobConfigurator, bool replaceJob = false) where T : class
        {
            ConfigureJob(configurator, jobConfigurator, replaceJob);
            return configurator;
        }

        private static void ConfigureJob<T>(ServiceConfigurator<T> configurator, Action<QuartzConfigurator> jobConfigurator, bool replaceJob = false) where T : class
        {
            var log = HostLogger.Get(typeof(ScheduleJobServiceConfiguratorExtensions));

            var jobConfig = new QuartzConfigurator();
            jobConfigurator(jobConfig);

            if ((jobConfig.JobEnabled == null || jobConfig.JobEnabled()) && jobConfig.Job != null && jobConfig.Triggers != null)
            {
                var jobDetail = jobConfig.Job();
                var jobTriggers = jobConfig.Triggers.Select(triggerFactory => triggerFactory()).Where(trigger => trigger != null).ToArray();
                var jobListeners = jobConfig.JobListeners;
                var triggerListeners = jobConfig.TriggerListeners;
                var scheduleListeners = jobConfig.ScheduleListeners;

                async Task BeforeStartingServiceFunc()
                {
                    log.Debug("[Topshelf.Quartz] Scheduler starting up...");
                    if (_scheduler == null)
                        _scheduler = await GetScheduler();

                    if (_scheduler != null && jobDetail != null && jobTriggers.Any())
                    {
                        var triggersForJob = new HashSet<ITrigger>(jobTriggers).ToArray();
                        await _scheduler.ScheduleJob(jobDetail, triggersForJob, replaceJob);
                        log.Info($"[Topshelf.Quartz] Scheduled Job: {jobDetail.Key}");

                        foreach (var trigger in triggersForJob)
                        {
                            log.Info($"[Topshelf.Quartz] Job Schedule: {trigger} - Next Fire Time (local): {trigger.GetNextFireTimeUtc()?.ToLocalTime().ToString() ?? "none"}");
                        }

                        if (jobListeners.Any())
                        {
                            foreach (var listener in jobListeners)
                            {
                                var config = listener();
                                _scheduler.ListenerManager.AddJobListener(config.Listener, config.Matchers);
                                log.Info($"[Topshelf.Quartz] Added Job Listener: {config.Listener.Name}");
                            }
                        }

                        if (triggerListeners.Any())
                        {
                            foreach (var listener in triggerListeners)
                            {
                                var config = listener();
                                _scheduler.ListenerManager.AddTriggerListener(config.Listener, config.Matchers);
                                log.Info($"[Topshelf.Quartz] Added Trigger Listener: {config.Listener.Name}");
                            }
                        }

                        if (scheduleListeners.Any())
                        {
                            foreach (var listener in scheduleListeners)
                            {
                                var scheduleListener = listener();
                                _scheduler.ListenerManager.AddSchedulerListener(scheduleListener);
                                log.Info($"[Topshelf.Quartz] Added Schedule Listener: {scheduleListener.GetType()}");
                            }
                        }

                        await _scheduler.Start();
                        log.Info("[Topshelf.Quartz] Scheduler started...");
                    }
                }

                async Task BeforeStoppingServiceFunc()
                {
                    log.Debug("[Topshelf.Quartz] Scheduler shutting down...");
                    if (_scheduler != null)
                        if (!_scheduler.IsShutdown)
                            await _scheduler.Shutdown();
                    log.Info("[Topshelf.Quartz] Scheduler shut down...");
                }

                configurator.BeforeStartingService(async () => await BeforeStartingServiceFunc());
                configurator.BeforeStoppingService(async () => await BeforeStoppingServiceFunc());
            }
        }
    }
}
