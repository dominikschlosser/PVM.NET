using log4net;
using PVM.Core.Data;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class ParallelGatewayOperation<T> : IOperation<T> where T : ICopyable<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelGatewayOperation<T>));

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

            if (execution.CurrentNode.OutgoingTransitions.Count == 1)
            {
                var transition = execution.CurrentNode.OutgoingTransitions[0];
                transition.Executed = true;
                execution.Start(transition.Destination, execution.Data);
            }
            else
            {
                foreach (var outgoingTransition in execution.CurrentNode.OutgoingTransitions)
                {
                    Logger.InfoFormat("Split to '{0}'", outgoingTransition.Identifier);
                    execution.CreateChild(outgoingTransition.Destination);
                }
            }
        }
    }
}