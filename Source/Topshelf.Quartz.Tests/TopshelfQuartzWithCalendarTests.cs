using NUnit.Framework;
using Quartz;
using Quartz.Impl.Calendar;
using System;
using Topshelf.Common.Tests;

namespace Topshelf.Quartz.Tests
{
    [TestFixture]
    public class TopshelfQuartzWithCalendarTests
    {
        [SetUp]
        public void Setup()
        {
            SampleJob.HasRun = false;
        }

        [Test]
        public void TestCanScheduleJobAlongsideServiceWithCalendar()
        {
            const string calendarName = "holidayToday";
            var host = HostFactory.New(configurator =>
            {
                configurator.UseTestHost();
                configurator.Service<SampleService>(s =>
                {
                    s.AddCalendar(calendarName, GetCalendar());
                    s.ConstructUsing(settings => new SampleService());
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                    s.ScheduleQuartzJob(q => q.WithJob(() =>
                        JobBuilder.Create<SampleJob>().Build()).AddTrigger(() =>
                        TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithRepeatCount(0)).ModifiedByCalendar(calendarName).Build()));
                });
            });
            host.Run();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1.0));

            Assert.IsFalse(SampleJob.HasRun);
        }

        private static ICalendar GetCalendar()
        {
            var bankHolidayCalendar = new HolidayCalendar();
            bankHolidayCalendar.AddExcludedDate(DateTime.Today);
            return bankHolidayCalendar;
        }


    }
}
