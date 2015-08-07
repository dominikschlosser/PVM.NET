using System.Linq;
using Moq;
using NUnit.Framework;
using PVM.Core.Definition;

namespace PVM.Core.Test.Definition
{
    [TestFixture]
    public class WorkflowDefinitionBuilderTest
    {
        [Test]
        public void AddNode_CreatesExecutionWithSingleNode()
        {
            var workflowBuilder = new WorkflowDefinitionBuilder();

            var definition = workflowBuilder.AddNode(Mock.Of<INode>()).Build();

            Assert.That(definition.Nodes.Count(), Is.EqualTo(1));
        }
    }
}