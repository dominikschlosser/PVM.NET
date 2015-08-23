using System;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class DynamicOperation<T> : IOperation<T> where T : class, new()
    {
        private readonly Action<IExecution, T> action;

        public DynamicOperation(Action<IExecution, T> action)
        {
            this.action = action;
        }

        public void Execute(IExecution execution, T dataContext)
        {
            action(execution, dataContext);
        }

        public void Execute(IExecution execution)
        {
            action(execution, null);
        }
    }
}