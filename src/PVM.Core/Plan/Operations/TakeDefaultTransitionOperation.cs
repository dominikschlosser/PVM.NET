
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class TakeDefaultTransitionOperation<T> : IOperation<T>
    {
        public void Execute(IExecution<T> execution)
        {
            execution.Proceed();
        }
    }
}
