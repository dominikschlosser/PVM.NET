using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace PVM.Core.Inject
{
    public class BasicServiceLocator : ServiceLocatorImplBase
    {
        private readonly IDictionary<Type, object> objects = new Dictionary<Type, object>();

        public void Register(Type type, object obj)
        {
            objects.Add(type, obj);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (objects.ContainsKey(serviceType))
            {
                return objects[serviceType];
            }

            if (serviceType.IsClass && serviceType.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(serviceType);
            }

            throw new InvalidOperationException(string.Format("There is no registered object for type '{0}'",
                serviceType.FullName));
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            yield return DoGetInstance(serviceType, null);
        }
    }
}