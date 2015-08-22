using log4net;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelJoinOperation : IOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelJoinOperation));

        public void Execute(IExecution execution)
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