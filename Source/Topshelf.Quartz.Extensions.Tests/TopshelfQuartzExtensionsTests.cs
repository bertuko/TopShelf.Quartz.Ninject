﻿using NUnit.Framework;
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
            HostFactory.Run(configurator =>
                {
                    configurator.UseTestHost();
                    configurator.Service<SampleService>(s =>
                        {
                            s.ConstructUsing(settings => new SampleService());
                            s.WhenStarted((service, control) => service.Start());
                            s.WhenStopped((service, control) => service.Stop());
                            s.ScheduleQuartzJobs()
                                .WithSimpleSchedule<SampleJob>(builder => builder.WithRepeatCount(0));
                        });
                });

            Assert.IsTrue(HasJobRun.HasRun);
        }
    }
}
