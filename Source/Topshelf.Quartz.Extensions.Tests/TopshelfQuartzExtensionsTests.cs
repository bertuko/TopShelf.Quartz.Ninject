using NUnit.Framework;
using System;
using Topshelf.Common.Tests;

namespace Topshelf.Quartz.Extensions.Tests
{
    [TestFixture]
    public class TopshelfQuartzExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
            HasJobRun.HasRun = false;
        }

        [Test]
        public void TestCanScheduleJobAlongsideService()
        {
            var host = HostFactory.New(configurator =>
                {
                    configurator.UseTestHost();
                    configurator.Service<SampleService>(s =>
                        {
                            s.ConstructUsing(settings => new SampleService());
                            s.WhenStarted((service, control) => service.Start());
                            s.WhenStopped((service, control) => service.Stop());
                            s.ScheduleQuartzJobWithSimpleSchedule<SampleService, SampleJob>(builder => builder.WithRepeatCount(0));
                        });
                });
            host.Run();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(HasJobRun.HasRun);
        }
    }
}
