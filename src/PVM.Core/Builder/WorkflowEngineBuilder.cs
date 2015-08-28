using Microsoft.Practices.ServiceLocation;
using PVM.Core.Runtime;

namespace PVM.Core.Builder
{
    public class WorkflowEngineBuilder
    {
        public WorkflowEngine BuildWithCustomServiceLocator(IServiceLocator serviceLocator)
        {
            return new WorkflowEngine(serviceLocator);
        }

        public WorkflowEngine Build()
        {
            return WithNinject().Build();
        }

        public NinjectServiceLocatorBuilder WithNinject()
        {
            return new NinjectServiceLocatorBuilder();
        }
    }
}