using System;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class DynamicOperation : IOperation
    {
        private readonly Action<IExecution> action;

        public DynamicOperation(Action<IExecution> action)
        {
            this.action = action;
        }

        public void Execute(IExecution execution)
        {
            action(execution);
        }
    }
}