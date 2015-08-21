using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelGatewayOperation<T> : IOperation<T>
    {
        public void Execute(IExecution<T> execution)
        {
            execution.Stop();

            new ParallelJoinOperation<T>().Execute(execution);

            new ParallelSplitOperation<T>().Execute(execution);
        }
    }
}