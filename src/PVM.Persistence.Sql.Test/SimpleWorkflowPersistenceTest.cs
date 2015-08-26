using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Core.Data.Attributes;
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
                .BuildWorkflow<ITestData>();

            var instance = new WorkflowInstance(workflowDefinition, new SqlPersistenceProvider());

            instance.Start(new StartData());

            Assert.False(instance.IsFinished);

        }

        private class TestOperation : IOperation
        {
            public void Execute(IExecution execution)
            {
                execution.Wait("signal");
            }
        }

        private class StartData : ITestData
        {
            public StartData()
            {
                Counter = 0;
                Data = new NestedTestClass(){Name = "bla", Value = 42.3f};
            }

            public int Counter { get; set; }
            public NestedTestClass Data { get; set; }
        }

        [WorkflowData]
        public interface ITestData
        {
            [In]
            [Out]
            int Counter { get; set; }

            [In]
            [Out]
            NestedTestClass Data { get; set; }
        }

        public class NestedTestClass
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }
    }
}