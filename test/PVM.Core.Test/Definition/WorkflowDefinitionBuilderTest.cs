using System.Linq;
using Moq;
using NUnit.Framework;
using PVM.Core.Definition;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Test.Definition
{
    [TestFixture]
    public class WorkflowDefinitionBuilderTest
    {
        [Test]
        public void AddNode_CreatesDefinitionWithSingleNode()
        {
            var workflowBuilder = new WorkflowDefinitionBuilder();
            var node = Mock.Of<INode>();

            var definition = workflowBuilder.AddNode(node).Build();

            Assert.That(definition.Nodes.Count(), Is.EqualTo(1));
            Assert.That(definition.Nodes.Contains(node));
        }

        [Test]
        public void AddInitialNode_CreatesDefinitionWithInitalNode()
        {
            var workflowBuilder = new WorkflowDefinitionBuilder();
            var node = Mock.Of<INode>();

            var definition = workflowBuilder.AddInitialNode(node).Build();

            Assert.That(definition.InitialNode, Is.EqualTo(node));
            Assert.That(definition.Nodes.Count(), Is.EqualTo(1));
            Assert.That(definition.Nodes.Contains(node));
        }
    }
}