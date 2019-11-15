using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using Topshelf.Builders;
using Topshelf.Configurators;
using Topshelf.HostConfigurators;

namespace Topshelf.Ninject
{
    public class NinjectBuilderConfigurator : HostBuilderConfigurator
    {
        private static Lazy<IKernel> _kernel;
        public static IKernel Kernel => _kernel.Value;

        public NinjectBuilderConfigurator(INinjectSettings settings, INinjectModule[] modules)
        {
            _kernel = new Lazy<IKernel>(() => GetKernel(settings, modules), true);
        }

        public NinjectBuilderConfigurator(IKernel kernel)
        {
            _kernel = new Lazy<IKernel>(() => kernel, true);
        }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }

        private IKernel GetKernel(INinjectSettings settings, INinjectModule[] modules)
        {
            return settings != null ? new StandardKernel(settings, modules) : new StandardKernel(modules);
        }
    }
}