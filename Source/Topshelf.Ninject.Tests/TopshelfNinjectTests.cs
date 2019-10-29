using NUnit.Framework;
using Topshelf.Common.Tests;

namespace Topshelf.Ninject.Tests
{
    [TestFixture]
    public class TopshelfNinjectTests
    {
        [Test]
        public void TestCanInstantiateServiceWithNinject()
        {
            var hasStarted = false;
            var hasStopped = false;

            Host host = HostFactory.New(configurator =>
                                            {
                                                configurator.UseTestHost();
                                                configurator.UseNinject(new SampleNinjectModule());
                                                configurator.Service<SampleNinjectService>(s =>
                                                                                               {
                                                                                                   s.ConstructUsingNinject();
                                                                                                   s.WhenStarted((service, control) => hasStarted = service.Start());
                                                                                                   s.WhenStopped((service, control) => hasStopped = service.Stop());
                                                                                               });
                                            });

            TopshelfExitCode exitCode = host.Run();

            Assert.AreEqual(TopshelfExitCode.Ok, exitCode);
            Assert.IsTrue(hasStarted);
            Assert.IsTrue(hasStopped);
        }
    }
}
