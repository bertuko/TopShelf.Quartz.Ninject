using Ninject;
using Topshelf.Ninject;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz.Ninject
{
    public static class QuartzNinjectServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> UseNinjectQuartzJobFactory<T>(this ServiceConfigurator<T> configurator)
            where T : class
        {
            return configurator.UsingQuartzJobFactory(() => NinjectBuilderConfigurator.Kernel.Get<NinjectJobFactory>());
        }
    }
}
