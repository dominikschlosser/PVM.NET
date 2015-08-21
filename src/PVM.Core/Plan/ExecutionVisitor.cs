using System;
using PVM.Core.Data;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public class ExecutionVisitor<T> : IExecutionVisitor<T> where T : ICopyable<T>
    {
        private readonly Action<IExecution<T>> action;

        public ExecutionVisitor(Action<IExecution<T>> action)
        {
            this.action = action;
        }

        public void Visit(IExecution<T> execution)
        {
            action(execution);
        }
    }
}