
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations.Base
{
    public class TakeDefaultTransitionOperation : IOperation
    {
        public void Execute(IExecution execution)
        {
            execution.Proceed();
        }
    }
}
