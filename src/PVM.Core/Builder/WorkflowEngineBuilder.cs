using Microsoft.Practices.ServiceLocation;
using PVM.Core.Inject;
using PVM.Core.Runtime;

namespace PVM.Core.Builder
{
    public class WorkflowEngineBuilder
    {
        private IServiceLocator serviceLocator = new BasicServiceLocator();

        public WorkflowEngineBuilder()
        {
            WithBasicServiceLocator().Build();
        }

        public WorkflowEngineBuilder WithServiceLocator(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            return this;
        }

        public BasicServiceLocatorBuilder WithBasicServiceLocator()
        {
            serviceLocator = new BasicServiceLocator();
            return new BasicServiceLocatorBuilder((BasicServiceLocator) serviceLocator, this);
        }

        public WorkflowEngine Build()
        {
            return new WorkflowEngine(serviceLocator);
        }
    }
}