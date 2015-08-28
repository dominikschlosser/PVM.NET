using Ninject;
using Ninject.Modules;
using PVM.Core.Inject;
using PVM.Core.Persistence;
using PVM.Core.Runtime;
using PVM.Core.Serialization;

namespace PVM.Core.Builder
{
    public class NinjectServiceLocatorBuilder
    {
        private readonly IKernel kernel = new StandardKernel(new PvmModule());

        public NinjectServiceLocatorBuilder OverridePersistenceProvider<T>() where T : IPersistenceProvider
        {
            kernel.Rebind<IPersistenceProvider>().To<T>();
            return this;
        }

        public NinjectServiceLocatorBuilder OverrideObjectSerializer<T>() where T : IObjectSerializer
        {
            kernel.Rebind<IObjectSerializer>().To<T>();
            return this;
        }

        public NinjectServiceLocatorBuilder ImportModule(NinjectModule module)
        {
            kernel.Load(module);
            return this;
        }

        public WorkflowEngine Build()
        {
            return new WorkflowEngine(new NinjectServiceLocator(kernel));
        }
    }
}