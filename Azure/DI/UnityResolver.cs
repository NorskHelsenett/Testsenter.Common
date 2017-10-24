using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using Shared.Common.DI;
using System.Web.Http.Dependencies;

namespace Shared.AzureCommon.DI
{
    public class UnityResolver : System.Web.Mvc.IDependencyResolver, IDependencyResolver
    {
        protected IDependencyInjector Di;

        public UnityResolver(IDependencyInjector di)
        {
            Di = di;
        }

        //System.Web.Http.Dependencies.IDependencyResolver
        public IDependencyScope BeginScope()
        {
            return new UnityResolver(Di);
        }

        private static readonly HashSet<string> IgnoreThese = new HashSet<string> {
            "IModelValidatorCache",
            "ModelMetadataProvider",
            "IHttpControllerActivator",
            "IHttpControllerSelector",
            "IHttpActionSelector",
            "IHttpActionInvoker",
            "IHostBufferPolicySelector",
            "IExceptionHandler",
            "IContentNegotiator",
            "IControllerFactory"};

        public object GetService(Type serviceType)
        {
            try
            {
                if (IgnoreThese.Contains(serviceType.Name))
                    return null;

                return Di.GetInstance(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>(); //<- cant see why this is used ..
        }


        public void Dispose()
        {

        }

        
    }
}
