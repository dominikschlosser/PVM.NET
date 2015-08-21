using System;
using PVM.Core.Builder;
using PVM.Core.Data;
using PVM.Core.Definition.Nodes;
using PVM.Core.Plan;
using PVM.Core.Runtime;

namespace PVM.Core.Test
{
    public static class TestExtensions
    {
        public static IWorkflowPathBuilder<T> BuildMockNode<T>(this NodeBuilder<T> nodeBuilder,
            Action<bool> executeAction)
        {
            return nodeBuilder.BuildNode(n => new MockNode<T>(n, executeAction));
        }

        private class MockNode<T> : Node<T> { 
            private readonly Action<bool> action;

            public MockNode(string name, Action<bool> action) : base(name)
            {
                this.action = action;
            }

            public override void Execute(IExecution<T> execution, IExecutionPlan<T> executionPlan)
            {
                action(true);

                base.Execute(execution, executionPlan);
            }
        }
    }
}