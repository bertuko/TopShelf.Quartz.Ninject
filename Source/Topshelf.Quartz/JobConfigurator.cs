using Quartz;
using System;
using System.Collections.Generic;

namespace Topshelf.Quartz
{
    public class JobConfigurator
    {
        public Func<IJobDetail> Job { get; private set; }
        public IList<Func<ITrigger>> Triggers { get; }
        public Func<bool> JobEnabled { get; private set; }

        public JobConfigurator()
        {
            Triggers = new List<Func<ITrigger>>();
        }

        public JobConfigurator WithJob(Func<IJobDetail> jobDetail)
        {
            Job = jobDetail;
            return this;
        }

        public JobConfigurator AddTrigger(Func<ITrigger> jobTrigger)
        {
            Triggers.Add(jobTrigger);
            return this;
        }

        public JobConfigurator WithTriggers(IEnumerable<ITrigger> jobTriggers)
        {
            foreach (var jobTrigger in jobTriggers)
            {
                var trigger = jobTrigger;
                AddTrigger(() => trigger);
            }
            return this;
        }

        public JobConfigurator EnableJobWhen(Func<bool> jobEnabled)
        {
            JobEnabled = jobEnabled;
            return this;
        }
    }
}