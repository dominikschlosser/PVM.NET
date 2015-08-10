using System.Linq;
using log4net;
using PVM.Core.Definition.Executions;

namespace PVM.Core.Definition.Behaviors
{
    public class ParallelGatewayBehavior : IBehavior
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelGatewayBehavior));

        public void Execute(IExecution execution)
        {
            execution.Stop();
            if (execution.CurrentNode.IncomingTransitions.Any(t => !t.Executed))
            {
                string transitionsLeft =
                    execution.CurrentNode.IncomingTransitions.Where(t => !t.Executed).Select(t => t.Identifier)
                             .Aggregate((t1, t2) => t1 + ", " + t2);
                Logger.Info(
                    $"Parallel gateway is still waiting for all incoming transitions to come in. Transitions left: {transitionsLeft}");
                return;
            }

            foreach (var outgoing in execution.CurrentNode.OutgoingTransitions)
            {
                Logger.Info($"Creating new execution path: {outgoing.Destination.Name}");
                execution.CreateChild(outgoing.Destination);
            }
        }
    }
}