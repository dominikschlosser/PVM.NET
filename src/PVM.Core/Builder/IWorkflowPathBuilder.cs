using PVM.Core.Definition;

namespace PVM.Core.Builder
{
    public interface IWorkflowPathBuilder
    {
        NodeBuilder AddNode();
        WorkflowDefinition BuildWorkflow();
    }
}