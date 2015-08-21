using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public interface IOperation<T>
    {
        void Execute(IExecution<T> execution);
    }
}