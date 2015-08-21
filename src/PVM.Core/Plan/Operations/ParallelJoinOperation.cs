using log4net;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelJoinOperation<T> : IOperation<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelJoinOperation<T>));

        public void Execute(IExecution<T> execution)
        {
            execution.Stop();

            foreach (var incomingTransition in execution.CurrentNode.IncomingTransitions)
            {
                if (!incomingTransition.Executed)
                {
                    Logger.InfoFormat("Transition '{0}' not taken yet. Waiting...", incomingTransition.Identifier);
                    return;
                }
            }

            execution.Resume();
        }
    }
}