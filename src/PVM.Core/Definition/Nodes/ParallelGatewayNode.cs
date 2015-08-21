using PVM.Core.Data;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Definition.Nodes
{
    public class ParallelGatewayNode<T> : Node<T> where T : ICopyable<T>
    {
        public ParallelGatewayNode(string name) : base(name)
        {
        }

        public override void Execute(IExecution<T> execution, IExecutionPlan<T> executionPlan)
        {
            executionPlan.Proceed(execution, new ParallelGatewayOperation<T>());
        }
    }
}