using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public interface IOperation
    {
        void Execute(IExecution execution);
    }
}