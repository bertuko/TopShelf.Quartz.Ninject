using Quartz;
using System.Threading.Tasks;

namespace Topshelf.Common.Tests
{
    public static class HasJobRun
    {
        public static bool HasRun;
    }

    public class SampleJob : IJob
    {

        protected SampleDependency Dependency;

        public SampleJob()
        {
            Dependency = new SampleDependency();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Dependency.DoWork();
            HasJobRun.HasRun = true;
        }
    }

    public class SampleNinjectJob : SampleJob
    {
        public SampleNinjectJob(SampleDependency dependency)
        {
            Dependency = dependency;
        }
    }
}