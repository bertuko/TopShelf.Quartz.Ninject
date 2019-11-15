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

            var exitCode = HostFactory.Run(configurator =>
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

            Assert.AreEqual(TopshelfExitCode.Ok, exitCode);
            Assert.IsTrue(hasStarted);
            Assert.IsTrue(hasStopped);
        }
    }
}
