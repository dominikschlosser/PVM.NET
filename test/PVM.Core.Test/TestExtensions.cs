using System;
using PVM.Core.Builder;
using PVM.Core.Definition;
using PVM.Core.Plan;
using PVM.Core.Runtime;

namespace PVM.Core.Test
{
    public static class TestExtensions
    {
        public static IWorkflowPathBuilder BuildMockNode(this NodeBuilder nodeBuilder,
            Action<bool> executeAction)
        {
            return nodeBuilder.BuildNode(n => new MockNode(n, executeAction));
        }

        private class MockNode : Node
        {
            private readonly Action<bool> action;

            public MockNode(string name, Action<bool> action) : base(name)
            {
                this.action = action;
            }

            public override void Execute(IInternalExecution execution, IExecutionPlan executionPlan)
            {
                action(true);

                base.Execute(execution, executionPlan);
            }
        }
    }
}