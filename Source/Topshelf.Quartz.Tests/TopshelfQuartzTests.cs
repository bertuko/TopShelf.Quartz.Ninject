using NUnit.Framework;
using Quartz;
using Quartz.Impl.Calendar;
using System;
using Topshelf.Common.Tests;

namespace Topshelf.Quartz.Tests
{
    [TestFixture]
    public class TopshelfQuartzTests
    {
        [SetUp]
        public void Setup()
        {
            HasJobRun.HasRun = false;
        }

        [Test]
        public void TestCanScheduleJobAlongsideService()
        {
            const string calendarName = "calendar";
            HostFactory.Run(configurator =>
                {
                    configurator.UseTestHost();
                    configurator.Service<SampleService>(s =>
                    {
                        s.ConfigureQuartzScheduler(() => new SchedulerConfigurator()
                            .WithCalendar(() => new QuartzCalendarConfig(calendarName, GetCalendar()))
                        );
                        s.ConstructUsing(settings => new SampleService());
                        s.WhenStarted((service, control) => service.Start());
                        s.WhenStopped((service, control) => service.Stop());
                        s.ScheduleQuartzJob(q =>
                            q.WithJob(() =>
                                JobBuilder.Create<SampleJob>().Build()).AddTrigger(() =>
                                TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithRepeatCount(0)).ModifiedByCalendar(calendarName).Build()));
                    }).StartAutomatically();
                });

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(HasJobRun.HasRun);
        }

        private static ICalendar GetCalendar()
        {
            var bankHolidayCalendar = new HolidayCalendar();
            bankHolidayCalendar.AddExcludedDate(DateTime.Today.AddDays(1));
            return bankHolidayCalendar;
        }

    }
}
