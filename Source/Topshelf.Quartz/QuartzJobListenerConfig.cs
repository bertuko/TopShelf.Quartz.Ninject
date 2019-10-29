using Quartz;
using System.Collections.Generic;

namespace Topshelf.Quartz
{
    public class QuartzJobListenerConfig
    {
        public IJobListener Listener { get; set; }
        public IReadOnlyCollection<IMatcher<JobKey>> Matchers { get; set; }

        public QuartzJobListenerConfig(IJobListener listener, params IMatcher<JobKey>[] matchers)
        {
            Listener = listener;
            Matchers = matchers;
        }

    }
}