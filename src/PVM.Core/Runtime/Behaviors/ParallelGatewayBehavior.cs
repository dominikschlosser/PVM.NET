using log4net;
using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;

namespace PVM.Core.Runtime.Behaviors
{
    public class ParallelGatewayBehavior<T> : IBehavior<T> where T : IProcessData<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ParallelGatewayBehavior<T>));

        public IOperation<T> CreateOperation(INode<T> node)
        {
            Logger.InfoFormat("Execution parallel gateway node '{0}'", node.Name);
            return new ParallelGatewayOperation<T>();
        }
    }
}