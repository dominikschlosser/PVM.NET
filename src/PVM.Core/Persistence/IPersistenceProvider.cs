using PVM.Core.Definition;
using PVM.Core.Runtime;

namespace PVM.Core.Persistence
{
    public interface IPersistenceProvider
    {
        void Persist(IExecution execution);
        void Persist(IWorkflowDefinition workflowDefinition);
    }
}