using Microsoft.Practices.Unity;

namespace Shared.Common.DI
{
    public enum InstanceLifetime
    {
        ReturnNewInstanceForEachResolve = 0, //TransientLifetimeManager
        ReturnSameInstanceForEachResolve = 1, //ContainerControlledLifetimeManager 
        ReturnNewInstanceForEachThread = 2 //PerThreadLifetimeManager 
    }

    internal static class ConvertInstanceLifeTime
    {
        internal static LifetimeManager AsLifetimeManager(this InstanceLifetime instanceLifeTime)
        {
            switch (instanceLifeTime)
            {
                case InstanceLifetime.ReturnNewInstanceForEachResolve:
                    return new TransientLifetimeManager();
                case InstanceLifetime.ReturnNewInstanceForEachThread:
                    return new PerThreadLifetimeManager();
                case InstanceLifetime.ReturnSameInstanceForEachResolve:
                    return new ContainerControlledLifetimeManager();
            }

            return new TransientLifetimeManager();
        }
    }
}
