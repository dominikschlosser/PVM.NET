using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class ParallelWorkflow
    {
        [Test]
        public void SingleBranch_ExecutesNodeAfterJoin()
        {
            var builder = new WorkflowDefinitionBuilder();
            bool executed = false;

            var workflowDefinition = builder
                .AddNode()
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
                .BuildParallelGateway()
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
                        .AddTransition()
                                    .WithName("joinToEnd")
                                    .To("end")
                                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildMockNode(e => executed = e)
                .BuildWorkflow();

            var instance = workflowDefinition.CreateNewInstance();
            instance.Start();

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }
    }
}