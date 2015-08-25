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

            var owningExecution = execution.Parent ?? execution;

            if (execution.CurrentNode.OutgoingTransitions.Count == 1)
            {
                owningExecution.Resume(execution.CurrentNode);
            }
            else
            {
                foreach (var outgoingTransition in execution.CurrentNode.OutgoingTransitions)
                {
                    outgoingTransition.Executed = true;
                    Logger.InfoFormat("Split to '{0}'", outgoingTransition.Identifier);
                    owningExecution.CreateChild(outgoingTransition.Destination);
                }
            }
        }
    }
}