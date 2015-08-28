using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace PVM.Core.Inject
{
    public class NinjectServiceLocator : ServiceLocatorImplBase, IDisposable
    {
        private IKernel kernel;

        public NinjectServiceLocator(IKernel kernel)
        {
            this.kernel = kernel;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key == null)
            {
                return kernel.Get(serviceType);
            }
            return kernel.Get(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        public void Dispose()
        {
            if (kernel != null && !kernel.IsDisposed)
            {
                kernel.Dispose();
                kernel = null;
            }
        }
    }
}