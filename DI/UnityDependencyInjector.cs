using Microsoft.Practices.Unity;
using Shared.Common.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Common.DI
{
    public class UnityDependencyInjector : IDependencyInjector
    {
        private bool _disposing;
        public UnityContainer UnityContainer;
        public ServiceDescription InstanceDescription { get; set; }

        public UnityDependencyInjector(ServiceDescription caller)
        {
            UnityContainer = new UnityContainer();
            RegisterInstance<IDependencyInjector>(this);
            _disposing = false;
            InstanceDescription = caller;
        }

        public virtual void Dispose()
        {
            if (_disposing)
                return;

            _disposing = true;
            UnityContainer.Dispose();
        }

        #region Resolve
        public TInterface GetInstance<TInterface>(string type = "") 
        {
            if (string.IsNullOrEmpty(type))
                return UnityContainer.Resolve<TInterface>();

            return (TInterface)UnityContainer.Resolve(typeof(TInterface), type);
        }

        public TInterface GetInstance<TInterface>(Enum type)
        {
            return (TInterface)UnityContainer.Resolve(typeof(TInterface), type.ToString());
        }

        public TInterface GetInstance<TInterface>(Dictionary<string, object> constructorParameterOverrides)
        {
            var overrides = constructorParameterOverrides.Select(x => new ParameterOverride(x.Key, x.Value));
            return UnityContainer.Resolve<TInterface>(overrides.ToArray());
        }

        public TInterface GetInstance<TInterface>(string type, Dictionary<string, object> constructorParameterOverrides)
        {
            var overrides = constructorParameterOverrides.Select(x => new ParameterOverride(x.Key, x.Value));

            return (TInterface)UnityContainer.Resolve(typeof(TInterface), type, overrides.ToArray());
        }

        public object GetInstance(Type interfaceType)
        {
            return UnityContainer.Resolve(interfaceType);
        }

        public IEnumerable<TInterface> GetAllInstancesOf<TInterface>()
        {
            return UnityContainer.ResolveAll<TInterface>();
        }

        public IEnumerable<object> GetAllInstancesOf(Type t)
        {
            return UnityContainer.ResolveAll(t);
        }

        #endregion

        #region Register

        public void RegisterInstance<TInterface>(object instance, string name)
        {
            UnityContainer.RegisterInstance(typeof(TInterface), name, instance, new ContainerControlledLifetimeManager());
        }

        public void RegisterInstance<TInterface>(object instance)
        {
            UnityContainer.RegisterInstance(typeof(TInterface), instance, new ContainerControlledLifetimeManager());
        }

        public void Register<TInterface, TImplementation>(InstanceLifetime lifeTime = InstanceLifetime.ReturnNewInstanceForEachResolve, string name = "") where TImplementation : TInterface
        {
            UnityContainer.RegisterType<TInterface, TImplementation>(name, lifeTime.AsLifetimeManager());
        }

        public void Register<TInterface, TImplementation>(InstanceLifetime lifeTime, string name, params object[] constructorArguments) where TImplementation : TInterface
        {
            UnityContainer.RegisterType(
                typeof(TInterface),
                typeof(TImplementation),
                name,
                lifeTime.AsLifetimeManager(),
                GetConstructorInjection(constructorArguments));
        }

        protected void RegisterFactory<T>(InjectionFactory x, string name)
        {
            UnityContainer.RegisterType<T>(name, x);
        }

        public void Register<TInterface, TImplementation>(InstanceLifetime lifeTime, params object[] constructorArguments)
        {
            UnityContainer.RegisterType(
                typeof(TInterface),
                typeof(TImplementation),
                lifeTime.AsLifetimeManager(),
                GetConstructorInjection(constructorArguments));
        }

        private InjectionMember[] GetConstructorInjection(object[] constructorArguments)
        {
            if (constructorArguments == null || !constructorArguments.Any())
                return null;

            var tmp = new object[constructorArguments.Length];
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                var obj = constructorArguments[i];
                if (obj.GetType() == typeof(DiResolveArg))
                {
                    var cast = (DiResolveArg)obj;
                    tmp[i] = new ResolvedParameter(cast.Type, cast.Name);
                }
                else
                    tmp[i] = obj;
            }

            return new InjectionMember[] { new InjectionConstructor(tmp) };
        }



        #endregion

        #region Settings

        

        public RunningEnvironmentEnum GetRunningEnvironment()
        {
            return InstanceDescription.RunningEnvironment;
        }

        



        #endregion

    }
}
