
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class TakeDefaultTransitionOperation : IOperation
    {
        public void Execute(IExecution execution)
        {
            execution.Proceed();
        }
    }
}
