using System;

namespace Topshelf.Common.Tests
{
    public class SampleService
    {
        public bool Start()
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3.0));
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}