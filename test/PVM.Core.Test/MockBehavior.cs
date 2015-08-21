using PVM.Core.Definition;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Test
{
    public class MockBehavior : IBehavior
    {
        public bool Executed { get { return operation.Executed; } }

        private readonly MockOperation operation = new MockOperation();

        public IOperation CreateOperation(INode node)
        {
            return operation;
        }

        private class MockOperation : IOperation
        {
            public bool Executed { get; private set; }

            public void Execute(IExecution execution)
            {
                Executed = true;
                execution.Proceed();
            }
        }
    }
}