using PVM.Core.Definition.Executions;

namespace PVM.Core.Definition
{
    public interface IBehavior
    {
        void Execute(IExecution execution);
    }
}