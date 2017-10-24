using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;
using Newtonsoft.Json;
using Shared.Common.DI;
using Shared.Common.Storage;
using Shared.Common.Storage.Queue;

namespace Shared.AzureCommon.Scheduler
{
    public class ScheduledAutoTestApi : IScheduledAutoTestApi
    {
        private readonly SchedulerClient _schedulerClient;
        private const string CloudService = "CS-NorthEurope-scheduler";
        private readonly string _storageAccountName;
        private readonly IQueue _queue;
        private readonly int _minutesToKeepInCache;
        private readonly string _sasToken;

        #region Caching

        private static object _cacheLock = new object();
        private bool _needRefresh;
        private DateTime _nextRefresh;
        private Dictionary<string, Job> _cache;
        private Dictionary<string, Job> Cache
        {
            get
            {
                if(_cache == null || _nextRefresh.CompareTo(DateTime.Now) < 0 || _needRefresh)
                {
                    lock(_cacheLock)
                    {
                        if (_cache == null || _nextRefresh.CompareTo(DateTime.Now) < 0 || _needRefresh)
                        {
                            _cache = new Dictionary<string, Job>();
                            foreach (var job in _schedulerClient.Jobs.List(new JobListParameters()))
                            {
                                _cache.Add(job.Id, job);
                            }

                            _nextRefresh = DateTime.Now.AddMinutes(_minutesToKeepInCache);
                            _needRefresh = false;
                        }
                     }
                }

                return _cache;
            }
        }

        #endregion

        public ScheduledAutoTestApi(string subscriptionId, X509Certificate2 managementCertificate, IQueue queue, string jobCollectionName, string storageAccountName, string sasToken, int minutesToKeepInCache)
        {
            var credentials = new CertificateCloudCredentials(subscriptionId, managementCertificate);
            _schedulerClient = new SchedulerClient(CloudService, jobCollectionName, credentials);
            _queue = queue;
            _storageAccountName = storageAccountName;
            _sasToken = sasToken;
            _minutesToKeepInCache = minutesToKeepInCache;
            _needRefresh = false;
        }

        public IEnumerable<ScheduledAutoTest> GetAll()
        {
            return Cache.Values.Select(GetFromJob);
        }

        public ScheduledAutoTest Get(string name)
        {
            return GetFromJob(Cache[name]);
        }

        public ScheduledAutoTest Create(ScheduledAutoTest vm, string qName)
        {
            if (vm.NextRun != null)
                return Create(vm.Name, vm.TestPlanId, (DateTime) vm.NextRun, qName, vm.RecurringEveryXHour);

            throw new ArgumentException("vm.NextRun is null");
        }

        private ScheduledAutoTest Create(string name, int testplanId, DateTime startTime, string qName, int recurEveryXHour = 24)
        {
            name = name.Replace(" ", "");

            var action = new JobAction()
            {
                Type = JobActionType.StorageQueue,
                QueueMessage = new JobQueueMessage
                {
                    Message = GetRegTestBody(testplanId),
                    QueueName = qName,
                    StorageAccountName = _storageAccountName,
                    SasToken = _sasToken
                }
            };

            var jobRecurrence = new JobRecurrence()
            {
                EndTime = DateTime.Now.AddYears(1),
                Frequency = JobRecurrenceFrequency.Hour,
                Interval = recurEveryXHour
            };

            var jobCreateOrUpdateParameters = new JobCreateOrUpdateParameters()
            {
                Action = action,
                Recurrence = jobRecurrence,
                StartTime = startTime
            };

            var result = _schedulerClient.Jobs.CreateOrUpdate(name, jobCreateOrUpdateParameters);
            _needRefresh = true;

            return GetFromJob(result.Job);
        }

        public ScheduledAutoTest UpdateJob(string name, DateTime startTime, int recurEveryXHour)
        {
            var existingjob = _schedulerClient.Jobs.Get(name);

            var jobRecurrence = new JobRecurrence()
            {
                EndTime = DateTime.Now.AddYears(1),
                Frequency = JobRecurrenceFrequency.Hour,
                Interval = recurEveryXHour
            };

            var jobCreateOrUpdateParameters = new JobCreateOrUpdateParameters()
            {
                Recurrence = jobRecurrence,
                StartTime = startTime,
                Action = existingjob.Job.Action
            };

            var result = _schedulerClient.Jobs.CreateOrUpdate(name, jobCreateOrUpdateParameters);
            _needRefresh = true;
            return GetFromJob(result.Job);
        }

        //public void StartJobNow(int testPlanId)
        //{
        //    StartTest.StartRegresjonsTest(_queue, testPlanId, TfsStates.TestCaseReady);
        //}

        public void DeleteJob(string name)
        {
            _schedulerClient.Jobs.Delete(name);
            _needRefresh = true;
        }

        //public static void Register(UnityDependencyInjector di, IQueue queueToTriggerNewStartRun, string jobCollectionName, string storageAccountName, string sasToken, Func<string, X509Certificate2> customCertificateProvider, string customCertificateProviderParameter)
        //{
        //    var managementCertificate = customCertificateProvider(customCertificateProviderParameter);
        //    string subscriptionId = di.GetConfigurationSetting(Configurationsetting.Common.AzureSubscriptionId);
        //    var api = new ScheduledAutoTestApi(subscriptionId, managementCertificate, queueToTriggerNewStartRun, jobCollectionName, storageAccountName, sasToken, 60);

        //    di.RegisterInstance<IScheduledAutoTestApi>(api);
        //}

        #region Helpers
        private ScheduledAutoTest GetFromJob(Job job)
        {
            return new ScheduledAutoTest
            {
                Name = job.Id,
                TestPlanId = GetTestPlanIdFromBody(job.Action.QueueMessage.Message),
                LastRun = job.Status.LastExecutionTime,
                NextRun = job.Status.NextExecutionTime,
                // ReSharper disable once PossibleInvalidOperationException
                RecurringEveryXHour = (int)job.Recurrence.Interval
            };
        }

        private int GetTestPlanIdFromBody(string queueBody)
        {
            try
            {
                queueBody = queueBody.Replace('"', ' ');
                queueBody = queueBody.Replace("/", "");
                queueBody = queueBody.Replace("\\", "");
                queueBody = queueBody.Replace(" ", "");
                var start = queueBody.IndexOf("Item1:", StringComparison.Ordinal) + 5;
                var stop = queueBody.IndexOf(",Item2", StringComparison.Ordinal);

                var x = queueBody.Substring(start + 1, stop - start - 1);

                return int.Parse(x);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private string GetRegTestBody(int testPlanId)
        {
            var obj = Queue.AzureQueue.GetContentAsQueueItem(new SimpleQueueItem(QueueItemType.RegresjonJob, "") { Content = new Tuple<int, string>(testPlanId, "Ready") });
            return JsonConvert.SerializeObject(obj);
        }

        
        #endregion

    }
}
