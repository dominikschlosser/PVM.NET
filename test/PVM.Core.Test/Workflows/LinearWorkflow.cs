using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Data;
using PVM.Core.Plan;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class LinearWorkflow
    {
        [Test]
        public void JustOneNode()
        {
            var builder = new WorkflowDefinitionBuilder<EmptyProcessData>();
            bool executed = false;

            var workflowDefinition =
                builder.AddNode()
                            .WithName("start")
                            .IsStartNode()
                            .IsEndNode()
                            .BuildMockNode(e => executed = e)
                       .BuildWorkflow();

            new WorkflowInstance<EmptyProcessData>(workflowDefinition).Start(new EmptyProcessData());

            Assert.That(executed);
        }

        [Test]
        public void SingleStartAndEndNode()
        {
            var builder = new WorkflowDefinitionBuilder<EmptyProcessData>();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .AddTransition()
                        .WithName("transition")
                        .To("end")
                        .BuildTransition()
                    .BuildMockNode(e => executed = e)
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                    .BuildNode()
                .BuildWorkflow();

            new WorkflowInstance<EmptyProcessData>(workflowDefinition).Start(new EmptyProcessData());

            Assert.That(executed);
        }
    }
}