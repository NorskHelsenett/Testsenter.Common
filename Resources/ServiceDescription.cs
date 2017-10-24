using System;

namespace Shared.Common.Resources
{
    public class ServiceDescription
    {
        public ServiceDescription(string service, RunningEnvironmentEnum runningEnvironment, bool onInternett, bool inAzure)
        {
            Service = service;
            OnInternett = onInternett;
            RunningInAzure = inAzure;
            Started = DateTime.UtcNow;
            RunningEnvironment = runningEnvironment;
        }

        public ServiceDescription(Enum service, RunningEnvironmentEnum runningEnvironment, bool onInternett, bool inAzure)
            : this(service.ToString(), runningEnvironment, onInternett, inAzure)
        {
        }

        public string Service { get; set; }
        public bool OnInternett { get; set; }
        public bool RunningInAzure { get; set; }
        public DateTime Started { get; set; }
        public RunningEnvironmentEnum RunningEnvironment { get; set; }

        public bool MockTfsWriteIntegration { get; set; }

        public override string ToString()
        {
            return $"Service: {Service.ToString()}, RunningEnvironment: {RunningEnvironment.ToString()}, in Azure: {RunningInAzure}";
        }
    }
}
