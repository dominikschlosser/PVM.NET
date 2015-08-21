using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Data;
using PVM.Core.Plan;
using PVM.Core.Runtime.Behaviors;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class ParallelWorkflow
    {
        [Test]
        public void SingleBranch_ExecutesNodeAfterJoin()
        {
            var builder = new WorkflowDefinitionBuilder<EmptyProcessData>();
            var behavior = new MockBehavior<EmptyProcessData>();

            var workflowDefinition = builder
                .AddNode()
                    .WithBehavior(new ParallelGatewayBehavior<EmptyProcessData>())
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
                    .WithBehavior(new ParallelGatewayBehavior<EmptyProcessData>())
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

            new WorkflowInstance<EmptyProcessData>(workflowDefinition).Start(new EmptyProcessData());

            Assert.That(behavior.Executed);
        }
    }
}