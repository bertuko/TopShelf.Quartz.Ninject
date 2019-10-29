using Ninject;
using Ninject.Syntax;
using Quartz;
using Quartz.Spi;

namespace Topshelf.Quartz.Ninject
{
    internal class NinjectJobFactory : IJobFactory
    {
        private readonly IResolutionRoot _resolutionRoot;

        public NinjectJobFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_resolutionRoot.Get(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            _resolutionRoot.Release(job);
        }
    }
}