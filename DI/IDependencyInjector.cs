using Shared.Common.Resources;
using System;
using System.Collections.Generic;

namespace Shared.Common.DI
{
    public interface IDependencyInjector : IDisposable
    {
        RunningEnvironmentEnum GetRunningEnvironment();
        TInterface GetInstance<TInterface>(string type = "");
        TInterface GetInstance<TInterface>(Enum type);
        TInterface GetInstance<TInterface>(Dictionary<string, object> constructorParameterOverrides);
        TInterface GetInstance<TInterface>(string type, Dictionary<string, object> constructorParameterOverrides);
        object GetInstance(Type interfaceType);
        IEnumerable<TInterface> GetAllInstancesOf<TInterface>();


        // ReSharper disable once MethodOverloadWithOptionalParameter
        void RegisterInstance<TInterface>(object instance, string name = "");
        void RegisterInstance<TInterface>(object instance);
        void Register<TInterface, TImplementation>(InstanceLifetime lifeTime = InstanceLifetime.ReturnNewInstanceForEachResolve, string name = "") where TImplementation : TInterface;
        void Register<TInterface, TImplementation>(InstanceLifetime lifeTime, string name, params object[] constructorArguments) where TImplementation : TInterface;
        void Register<TInterface, TImplementation>(InstanceLifetime lifeTime, params object[] constructorArguments);


    }
}
