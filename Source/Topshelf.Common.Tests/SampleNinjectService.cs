namespace Topshelf.Common.Tests
{
    public class SampleNinjectService
    {
        private readonly ISampleDependency _dependency;

        public SampleNinjectService(ISampleDependency dependency)
        {
            _dependency = dependency;
        }

        public bool Start()
        {
            _dependency.DoWork();
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}