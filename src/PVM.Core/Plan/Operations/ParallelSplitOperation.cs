using log4net;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelSplitOperation<T> : IOperation<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelSplitOperation<T>));

        public void Execute(IExecution<T> execution)
        {
            execution.Stop();

            if (execution.CurrentNode.OutgoingTransitions.Count == 1)
            {
                execution.Resume();
            }
            else
            {
                foreach (var outgoingTransition in execution.CurrentNode.OutgoingTransitions)
                {
                    outgoingTransition.Executed = true;
                    Logger.InfoFormat("Split to '{0}'", outgoingTransition.Identifier);
                    execution.CreateChild(outgoingTransition.Destination);
                }
            }
        }
    }
}