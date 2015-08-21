using PVM.Core.Data;
using PVM.Core.Definition;

namespace PVM.Core.Builder
{
    public interface IWorkflowPathBuilder<T> where T: ICopyable<T>
    {
        NodeBuilder<T> AddNode();
        WorkflowDefinition<T> BuildWorkflow();
    }
}