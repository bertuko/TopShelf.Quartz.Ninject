using Ninject;
using Ninject.Modules;
using Topshelf.HostConfigurators;
using Topshelf.Logging;

namespace Topshelf.Ninject
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseNinject(this HostConfigurator configurator, params INinjectModule[] modules)
        {
            return UseNinject(configurator, null, modules);
        }

        public static HostConfigurator UseNinject(this HostConfigurator configurator, INinjectSettings settings, params INinjectModule[] modules)
        {
            var log = HostLogger.Get(typeof(HostConfiguratorExtensions));

            log.Info("[Topshelf.Ninject] Integration Started in host.");
            log.Debug($"Ninject kernel instantiated with {modules.Length} modules.");

            configurator.AddConfigurator(new NinjectBuilderConfigurator(settings, modules));
            return configurator;
        }

        public static HostConfigurator UseNinject(this HostConfigurator configurator, IKernel kernel)
        {
            var log = HostLogger.Get(typeof(HostConfiguratorExtensions));

            log.Info("[Topshelf.Ninject] Integration Started in host.");

            configurator.AddConfigurator(new NinjectBuilderConfigurator(kernel));
            return configurator;
        }

    }
}