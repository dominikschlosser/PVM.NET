using PVM.Core.Data;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class TransientOperation<T> : IOperation<T> where T : IProcessData<T>
    {
        public void Execute(IExecution<T> execution)
        {
            execution.Proceed();
        }
    }
}
