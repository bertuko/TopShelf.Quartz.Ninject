using Ninject;
using Topshelf.Logging;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Ninject
{
    public static class NinjectServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> ConstructUsingNinject<T>(this ServiceConfigurator<T> configurator) where T : class
        {
            var log = HostLogger.Get(typeof(HostConfiguratorExtensions));

            log.Info("[Topshelf.Ninject] Service configured to construct using Ninject.");

            configurator.ConstructUsing(serviceFactory => NinjectBuilderConfigurator.Kernel.Get<T>());

            return configurator;
        }
    }
}