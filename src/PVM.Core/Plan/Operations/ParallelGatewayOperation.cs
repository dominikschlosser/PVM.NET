using log4net;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelGatewayOperation : IOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelGatewayOperation));

        public void Execute(IExecution execution)
        {
            execution.Stop();

            foreach (var incomingTransition in execution.CurrentNode.IncomingTransitions)
            {
                if (!incomingTransition.Executed)
                {
                    Logger.Info($"Transition '{incomingTransition.Identifier}' not taken yet. Waiting...");
                    return;
                }
            }

            if (execution.CurrentNode.OutgoingTransitions.Count == 1)
            {
                execution.Start(execution.CurrentNode.OutgoingTransitions[0].Destination);
            }
            else
            {
                foreach (var outgoingTransition in execution.CurrentNode.OutgoingTransitions)
                {
                    Logger.Info($"Split to '{outgoingTransition.Identifier}'");
                    execution.CreateChild(outgoingTransition.Destination);
                }
            }

        }
    }
}