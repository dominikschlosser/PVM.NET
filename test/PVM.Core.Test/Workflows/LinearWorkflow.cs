using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class LinearWorkflow
    {
        [Test]
        public void JustOneNode()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition =
                builder.AddNode()
                            .WithName("start")
                            .IsStartNode()
                            .IsEndNode()
                            .BuildMockNode(e => executed = e)
                       .BuildWorkflow();

            var instance = new WorkflowEngineBuilder().Build().CreateNewInstance(workflowDefinition);
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }

        [Test]
        public void SingleStartAndEndNode()
        {
            var builder = new WorkflowDefinitionBuilder();
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

            var instance = new WorkflowEngineBuilder().Build().CreateNewInstance(workflowDefinition);
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}