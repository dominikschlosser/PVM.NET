using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Definition;
using PVM.Core.Definition.Behaviors;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class ParallelWorkflow
    {
        [Test]
        public void SingleBranch_ExecutesNodeAfterJoin()
        {
            var builder = new WorkflowDefinitionBuilder();
            var behavior = new MockBehavior();

            var workflowDefinition = builder
                .AddNode()
                    .WithBehavior(new ParallelGatewayBehavior())
                    .WithName("split")
                    .IsStartNode()
                        .AddTransition()
                            .WithName("transition1")
                            .To("subNode1")
                            .BuildTransition()
                        .AddTransition()
                            .WithName("transition2")
                            .To("subNode2")
                            .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("subNode1")
                        .AddTransition()
                                .WithName("subNodeToEnd")
                                .To("join")
                                .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("subNode2")
                        .AddTransition()
                                .WithName("subNode2ToEnd")
                                .To("join")
                                .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("join")
                    .WithBehavior(new ParallelGatewayBehavior())
                        .AddTransition()
                                    .WithName("joinToEnd")
                                    .To("end")
                                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("end")
                    .WithBehavior(behavior)
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            new WorkflowInstance(workflowDefinition).Start();

            Assert.That(behavior.Executed);
        }
    }
}