using NUnit.Framework;
using Quartz;
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
            HasJobRun.HasRun = false;
        }

        [Test]
        public void TestCanScheduleJobAlongsideService()
        {
            HostFactory.Run(configurator =>
                {
                    configurator.UseTestHost();
                    configurator.UseNinject(new SampleNinjectModule());
                    configurator.Service<SampleNinjectService>(s =>
                        {
                            s.ConstructUsingNinject();
                            s.UseNinjectQuartzJobFactory();
                            s.WhenStarted((service, control) => service.Start(true));
                            s.WhenStopped((service, control) => service.Stop());
                            s.ScheduleQuartzJob(q =>
                                q.WithJob(() => JobBuilder.Create<SampleNinjectJob>().Build()).AddTrigger(() =>
                                    TriggerBuilder.Create().StartNow().Build()));
                        });
                });

            Assert.IsTrue(HasJobRun.HasRun);
        }
    }
}
