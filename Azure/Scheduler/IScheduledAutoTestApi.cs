using System;
using System.Collections.Generic;

namespace Shared.AzureCommon.Scheduler
{
    public interface IScheduledAutoTestApi
    {
        IEnumerable<ScheduledAutoTest> GetAll();
        ScheduledAutoTest Get(string name);
        ScheduledAutoTest Create(ScheduledAutoTest vm, string qName);
        ScheduledAutoTest UpdateJob(string name, DateTime startTime, int recurEveryXHour);
        void DeleteJob(string name);
        //void StartJobNow(int testPlanId);
    }
}
