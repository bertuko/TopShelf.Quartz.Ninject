using NUnit.Framework;
using Quartz;
using System;
using Topshelf.Common.Tests;
using Topshelf.Ninject;

namespace Topshelf.Quartz.Ninject.Tests
{
    [TestFixture]
    public class TopshelfQuartzNinjectTests
    {
        [SetUp]
        public void Setup()
        {
            SampleJob.HasRun = false;
        }

        [Test]
        public void TestCanScheduleJobAlongsideService()
        {
            var host = HostFactory.New(configurator =>
                {
                    configurator.UseTestHost();
                    configurator.UseNinject(new SampleNinjectModule());
                    configurator.Service<SampleNinjectService>(s =>
                        {
                            s.ConstructUsingNinject();
                            s.UseNinjectQuartzJobFactory();
                            s.WhenStarted((service, control) => service.Start());
                            s.WhenStopped((service, control) => service.Stop());
                            s.ScheduleQuartzJob(q =>
                                q.WithJob(() => JobBuilder.Create<SampleNinjectJob>().Build()).AddTrigger(() =>
                                    TriggerBuilder.Create().StartNow().Build()));
                        });
                });
            host.Run();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1.0));

            Assert.IsTrue(SampleJob.HasRun);
        }
    }
}
