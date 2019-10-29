using Quartz;
using System.Threading.Tasks;

namespace Topshelf.Common.Tests
{
    public class SampleJob : IJob
    {
        public static bool HasRun;

        protected SampleDependency Dependency;

        public SampleJob()
        {
            Dependency = new SampleDependency();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Dependency.DoWork();
            HasRun = true;
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