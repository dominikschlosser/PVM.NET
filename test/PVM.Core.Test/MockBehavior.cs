using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Test
{
    public class MockBehavior<T> : IBehavior<T> where T : IProcessData<T>
    {
        public bool Executed { get { return operation.Executed; } }

        private readonly MockOperation<T> operation = new MockOperation<T>();

        public IOperation<T> CreateOperation(INode<T> node)
        {
            return operation;
        }

        private class MockOperation<T> : IOperation<T> where T : IProcessData<T>
        {
            public bool Executed { get; private set; }

            public void Execute(IExecution<T> execution)
            {
                Executed = true;
                execution.Proceed();
            }
        }
    }
}