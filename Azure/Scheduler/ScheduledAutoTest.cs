using System;
using System.ComponentModel;
using Shared.Common.Logic;

namespace Shared.AzureCommon.Scheduler
{
    public class ScheduledAutoTest
    {
        public string Name { get; set; }
        public int TestPlanId { get; set; }

        

        [DisplayName("Timer mellom kjøringer")]
        public int RecurringEveryXHour { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? NextRun { get; set; }

        public string GetLastRun()
        {
            return TimeHelper.ToNorwegianText(LastRun);

        }
        public string GetNextRun()
        {
            return TimeHelper.ToNorwegianText(NextRun);
        }
    }
}
