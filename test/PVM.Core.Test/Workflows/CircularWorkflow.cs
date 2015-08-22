using log4net;
using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan;

namespace PVM.Core.Test.Workflows
{
    [TestFixture]
    public class CircularWorkflow
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (CircularWorkflow));

        [Test]
        public void Executes()
        {
            var builder = new WorkflowDefinitionBuilder<TestData>();
            bool executed = false;

            var workflowDefinition = builder.AddNode()
                .WithName("start")
                .IsStartNode()
                .AddTransition()
                    .WithName("toJoin")
                    .To("join")
                    .BuildTransition()
                .AddTransition()
                    .WithName("toIntermediate")
                    .To("intermediate")
                    .BuildTransition()
                .BuildParallelSplit()
                .AddNode()
                    .WithName("intermediate")
                    .AddTransition()
                        .WithName("intermediateToJoin")
                        .To("join")
                        .BuildTransition()
                    .AddTransition()
                        .WithName("intermediateToStart")
                        .To("start")
                        .BuildTransition()
                    .BuildDynamicNode(e =>
                    {
                        if (e.Data.Counter == 1)
                        {
                            Logger.Info("COUNTER == 1");
                            e.Proceed("intermediateToJoin");
                        }
                        else
                        {
                            Logger.Info("COUNTER == 0");
                            e.Data.Counter++;
                            e.Proceed("intermediateToStart");
                        }

                    })
                .AddNode()
                    .WithName("join")
                        .AddTransition()
                            .WithName("endTransition")
                            .To("end")
                            .BuildTransition()
                    .BuildParallelJoin()
                    .AddNode()
                        .WithName("end")
                        .IsEndNode()
                    .BuildMockNode(e => executed = e)
               .BuildWorkflow();

            var instance = new WorkflowInstance<TestData>(workflowDefinition);
            instance.Start(new TestData());

            Assert.That(executed);
            Assert.That(instance.IsFinished);
        }

        private class TestData
        {
            public int Counter { get; set; }
        }
    }
}