using PVM.Core.Definition;

namespace PVM.Core.Runtime
{
    public interface IWorkflowInstance : IExecution
    {
        IWorkflowDefinition Definition { get; }
    }
}