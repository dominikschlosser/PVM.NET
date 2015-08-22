using System;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public class ExecutionVisitor : IExecutionVisitor
    {
        private readonly Action<IExecution> action;

        public ExecutionVisitor(Action<IExecution> action)
        {
            this.action = action;
        }

        public void Visit(IExecution execution)
        {
            action(execution);
        }
    }
}