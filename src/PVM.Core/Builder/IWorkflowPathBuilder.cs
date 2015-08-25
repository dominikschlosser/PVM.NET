using PVM.Core.Definition;

namespace PVM.Core.Builder
{
    public interface IWorkflowPathBuilder
    {
        NodeBuilder AddNode();
        WorkflowDefinition<T> BuildWorkflow<T>() where T : class;
        WorkflowDefinition<object> BuildWorkflow();
        WorkflowDefinitionBuilder AsDefinitionBuilder();
    }
}