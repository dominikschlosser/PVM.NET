using PVM.Core.Data;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class TakeDefaultTransitionOperation<T> : IOperation<T> where T : ICopyable<T>
    {
        public void Execute(IExecution<T> execution)
        {
            execution.Proceed();
        }
    }
}
