using Quartz;
using System.Collections.Generic;

namespace Topshelf.Quartz
{
    public class QuartzTriggerListenerConfig
    {
        public ITriggerListener Listener { get; }
        public IReadOnlyCollection<IMatcher<TriggerKey>> Matchers { get; }
        public QuartzTriggerListenerConfig(ITriggerListener listener, params IMatcher<TriggerKey>[] matchers)
        {
            Listener = listener;
            Matchers = matchers;
        }
    }
}