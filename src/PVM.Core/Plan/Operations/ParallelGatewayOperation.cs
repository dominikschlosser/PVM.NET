using log4net;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelGatewayOperation : IOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ParallelGatewayOperation));

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

            new ParallelSplitOperation().Execute(execution);
        }
    }
}