using System;
using System.Threading;

namespace Topshelf.Common.Tests
{
    public class SampleNinjectService
    {
        private readonly ISampleDependency _dependency;

        public SampleNinjectService(ISampleDependency dependency)
        {
            _dependency = dependency;
        }

        public bool Start(bool sleep = false)
        {
            if (sleep)
                Thread.Sleep(TimeSpan.FromSeconds(3));

            _dependency.DoWork();
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}