using PVM.Core.Definition;
using PVM.Core.Runtime;

namespace PVM.Core.Persistence
{
    public class NullPersistenceProvider : IPersistenceProvider
    {
        public void Persist(IExecution execution)
        {
            // do nothing
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            // do nothing
        }
    }
}