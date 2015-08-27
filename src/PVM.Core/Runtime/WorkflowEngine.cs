using Microsoft.Practices.ServiceLocation;
using PVM.Core.Definition;

namespace PVM.Core.Runtime
{
    public class WorkflowEngine
    {
        private readonly IServiceLocator serviceLocator;

        public WorkflowEngine(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public WorkflowInstance CreateNewInstance(IWorkflowDefinition definition)
        {
            return new WorkflowInstance(definition, serviceLocator);
        }
    }
}
