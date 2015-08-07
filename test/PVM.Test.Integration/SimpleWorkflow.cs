using Moq;
using NUnit.Framework;
using PVM.Core.Definition;

namespace PVM.Test.Integration
{
	[TestFixture]
	public class SimpleWorkflow
	{
		[Test]
		public void SingleNode()
		{
			var builder = new WorkflowDefinitionBuilder();
			var executable = new Mock<IExecutable>();
			var workflowDefinition = builder.AddInitialNode(new NodeBuilder().WithExectuable(executable.Object).Build()).Build();

			new WorkflowInstance(workflowDefinition).Start();

			executable.Verify(e => e.Execute(It.IsAny<IExecution>()));
		}
	}
}