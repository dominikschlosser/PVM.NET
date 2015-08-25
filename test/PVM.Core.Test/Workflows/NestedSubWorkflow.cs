using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class NestedSubWorkflow
    {
        [Test]
        public void Executes()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .AddTransition()
                        .WithName("transition")
                        .To("startSub")
                        .BuildTransition()
                    .BuildNode()
                .AddNode()
                    .WithName("startSub")
                    .AddTransition()
                        .WithName("toEnd")
                        .To("end")
                        .BuildTransition()
                    .BuildSubWorkflow(new WorkflowDefinitionBuilder()
                    .AddNode()
                        .WithName("subStart")
                        .IsStartNode()
                        .AddTransition()
                            .WithName("subToEnd")
                            .To("subEnd")
                            .BuildTransition()
                        .BuildNode()
                    .AddNode()
                        .WithName("subEnd")
                        .IsEndNode()
                    .BuildNode()
                    .AsDefinitionBuilder())
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                    .BuildMockNode(e => executed = true)
                .BuildWorkflow();

            var instance = new WorkflowInstance(workflowDefinition);
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}