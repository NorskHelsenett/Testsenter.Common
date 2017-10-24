using Shared.Common.Resources;

namespace Shared.Common.DI
{
    public interface IExternalProjectRegistration 
    {
        void Register(UnityDependencyInjector di, ServiceDescription caller);
        void Dispose(UnityDependencyInjector di);
    }
}
