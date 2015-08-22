using PVM.Core.Builder;
using PVM.Core.Definition;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;
using System;

namespace PVM.Core.Test
{
    public static class TestExtensions
    {
        public static IWorkflowPathBuilder BuildMockNode(this NodeBuilder nodeBuilder,
            Action<bool> executeAction)
        {
            return nodeBuilder.BuildNode(n => new MockNode(n, executeAction));
        }

        public static IWorkflowPathBuilder BuildDynamicNode(this NodeBuilder nodeBuilder,
            Action<IExecution> action)
        {
            return nodeBuilder.BuildNode(n => new DynamicNode(n, action));
        }

        private class MockNode : Node { 
            private readonly Action<bool> action;

            public MockNode(string name, Action<bool> action) : base(name)
            {
                this.action = action;
            }

            public override void Execute(IExecution execution, IExecutionPlan executionPlan)
            {
                action(true);

                base.Execute(execution, executionPlan);
            }
        }

        private class DynamicNode : Node
        {
            private readonly Action<IExecution> action;

            public DynamicNode(string name, Action<IExecution> action) : base(name)
            {
                this.action = action;
            }

            public override void Execute(IExecution execution, IExecutionPlan executionPlan)
            {
                executionPlan.Proceed(execution, new DynamicOperation(action));
            }
        }
    }
}