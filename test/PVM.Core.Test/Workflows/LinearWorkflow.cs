using System.Collections.Generic;
using NUnit.Framework;
using PVM.Core.Definition;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class LinearWorkflow
    {
        [Test]
        public void JustOneNode()
        {
            var builder = new WorkflowDefinitionBuilder();
            var executable = new MockExecutable();
            var startNode = new NodeBuilder().WithExectuable(executable).Build();
            var workflowDefinition = builder.AddInitialNode(startNode).Build();

            new WorkflowInstance(workflowDefinition).Start();

            Assert.That(executable.Executed);
        }

        [Test]
        public void SingleStartAndEndNode()
        {
            var builder = new WorkflowDefinitionBuilder();
            var executable = new MockExecutable();
            var endNode = new EndNode("end", new List<Transition>());
            INode startNode = null;
            startNode =
                new NodeBuilder().WithExectuable(executable)
                    .WithOutgoingTransition(new Transition("t1", startNode, endNode))
                    .Build();
            var workflowDefinition = builder.AddInitialNode(startNode).AddEndNode(endNode).Build();

            new WorkflowInstance(workflowDefinition).Start();

            Assert.That(executable.Executed);
        }
    }
}