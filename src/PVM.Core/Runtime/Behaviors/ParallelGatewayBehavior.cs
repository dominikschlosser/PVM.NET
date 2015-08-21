using log4net;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;

namespace PVM.Core.Runtime.Behaviors
{
    public class ParallelGatewayBehavior : IBehavior
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ParallelGatewayBehavior));

        public IOperation CreateOperation(INode node)
        {
            Logger.InfoFormat("Execution parallel gateway node '{0}'", node.Name);
            return new ParallelGatewayOperation();
        }
    }
}