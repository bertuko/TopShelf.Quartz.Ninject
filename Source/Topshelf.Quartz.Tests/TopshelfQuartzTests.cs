using NUnit.Framework;
using Quartz;
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
            Host host = HostFactory.New(configurator =>
                                            {

                                                configurator.UseTestHost();
                                                configurator.Service<SampleService>(s =>
                                                                                        {
                                                                                            s.ConstructUsing(settings => new SampleService());
                                                                                            s.WhenStarted((service, control) => service.Start());
                                                                                            s.WhenStopped((service, control) => service.Stop());
                                                                                            s.ScheduleQuartzJob(q => q.WithJob(() => JobBuilder.Create<SampleJob>().Build()).AddTrigger(() => TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithRepeatCount(0)).Build()));
                                                                                        });
                                            });
            host.Run();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(HasJobRun.HasRun);
        }

    }
}
