using PVM.Core.Definition;

namespace PVM.Core.Builder
{
    public interface IWorkflowPathBuilder<T>
    {
        NodeBuilder<T> AddNode();
        WorkflowDefinition<T> BuildWorkflow();
    }
}