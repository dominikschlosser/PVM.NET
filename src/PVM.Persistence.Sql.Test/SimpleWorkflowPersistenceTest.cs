using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Persistence.Sql.Test
{
    [TestFixture]
    public class SimpleWorkflowPersistenceTest
    {
        [Test]
        public void PersistSingleExecution()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .WithOperation(new TestOperation())
                    .IsStartNode()
                    .AddTransition()
                        .WithName("transition")
                        .To("end")
                        .BuildTransition()
                    .BuildNode()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                    .BuildNode()
                .BuildWorkflow();

            var instance = new WorkflowInstance(workflowDefinition, new SqlPersistenceProvider());

            instance.Start();

            Assert.False(instance.IsFinished);

        }

        private class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Wait("signal");
            }
        }
    }
}