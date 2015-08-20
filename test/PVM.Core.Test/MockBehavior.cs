using PVM.Core.Definition;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Test
{
    public class MockBehavior : IBehavior
    {
        public bool Executed { get; private set; }

        public void Execute(INode node, IExecutionPlan executionPlan)
        {
            Executed = true;
            executionPlan.Proceed(node, new TransientOperation());
        }
    }
}