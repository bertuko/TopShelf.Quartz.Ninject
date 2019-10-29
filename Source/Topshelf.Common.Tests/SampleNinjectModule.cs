using Ninject.Modules;

namespace Topshelf.Common.Tests
{
    public class SampleNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISampleDependency>().To<SampleDependency>();
        }
    }
}

