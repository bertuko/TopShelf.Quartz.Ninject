using Ninject;
using Ninject.Modules;
using System.Collections.Generic;
using Topshelf.Builders;
using Topshelf.Configurators;
using Topshelf.HostConfigurators;

namespace Topshelf.Ninject
{
    public class NinjectBuilderConfigurator : HostBuilderConfigurator
    {
        private static INinjectSettings _settings;
        private static INinjectModule[] _modules;
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get
            {
                if (_kernel == null)
                    _kernel = _settings != null ? new StandardKernel(_settings, _modules) : new StandardKernel(_modules);
                return _kernel;
            }
        }

        public NinjectBuilderConfigurator(INinjectSettings settings, INinjectModule[] modules)
        {
            _settings = settings;
            _modules = modules;
        }

        public NinjectBuilderConfigurator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }
    }
}