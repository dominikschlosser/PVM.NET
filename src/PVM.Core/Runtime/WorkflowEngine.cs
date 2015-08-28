using System;
using Microsoft.Practices.ServiceLocation;
using PVM.Core.Definition;

namespace PVM.Core.Runtime
{
    public class WorkflowEngine : IDisposable
    {
        private IServiceLocator serviceLocator;

        public WorkflowEngine(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public WorkflowInstance CreateNewInstance(IWorkflowDefinition definition)
        {
            return new WorkflowInstance(definition, serviceLocator);
        }

        public void Dispose()
        {
            if (serviceLocator != null)
            {
                var disposable = serviceLocator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                serviceLocator = null;
            }
        }
    }
}
