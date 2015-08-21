using System;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class DynamicOperation<T> : IOperation<T>
    {
        private readonly Action<IExecution<T>> action;

        public DynamicOperation(Action<IExecution<T>> action)
        {
            this.action = action;
        }

        public void Execute(IExecution<T> execution)
        {
            action(execution);
        }
    }
}