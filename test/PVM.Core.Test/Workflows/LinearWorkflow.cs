using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Definition;

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
            var workflowDefinition =
                builder.AddNode()
                            .WithName("start")
                            .WithExecutable(executable)
                            .IsStartNode()
                            .IsEndNode()
                            .BuildNode()
                       .BuildWorkflow();

            new WorkflowInstance(workflowDefinition).Start();

            Assert.That(executable.Executed);
        }

        [Test]
        public void SingleStartAndEndNode()
        {
            var builder = new WorkflowDefinitionBuilder();
            var executable = new MockExecutable();

            var workflowDefinition = builder
                .AddNode()
                    .WithExecutable(executable)
                    .WithName("start")
                    .IsStartNode()
                    .AddTransition()
                        .WithName("transition")
                        .WithTarget("end")
                        .BuildTransition()
                    .BuildNode()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                    .BuildNode()
                .BuildWorkflow();

            new WorkflowInstance(workflowDefinition).Start();

            Assert.That(executable.Executed);
        }
    }
}